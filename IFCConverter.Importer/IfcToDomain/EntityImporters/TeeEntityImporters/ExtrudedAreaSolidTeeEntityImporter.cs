using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.TeeEntityImporters
{
    internal sealed class ExtrudedAreaSolidTeeEntityImporter : ITeeEntityImporter
    {
        private const double DoubleTolerance = 1e-6;
        private readonly VectorComparer _comparer = new VectorComparer(DoubleTolerance);

        public bool CanImport(IIfcProduct product)
        {
            if (!(product.ObjectPlacement is IIfcLocalPlacement))
                return false;
            IIfcRepresentationItem[] representationItems =
                product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 2)
                return false;
            
            foreach (IIfcRepresentationItem representationItem in representationItems)
            {
                if (!(representationItem is IIfcExtrudedAreaSolid extrudedAreaSolid))
                    return false;

                if (!(extrudedAreaSolid.SweptArea is IIfcCircleProfileDef))
                    return false;
            }

            return true;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            IIfcLocalPlacement localPlacement = (IIfcLocalPlacement)product.ObjectPlacement;
            IIfcExtrudedAreaSolid[] extrudedAreaSolids = product.GetRepresentationItems().Cast<IIfcExtrudedAreaSolid>().ToArray();

            double lengthPower = product.Model.GetLengthPower();
            FixedMatrix<Dim4> globalMatrix = localPlacement.GetGlobalMatrix();
            
            SolidGeometry[] solids = extrudedAreaSolids.Select(solid => CreateSolidGeometry(solid, globalMatrix)).ToArray();
            
            if (!TryGetIntersection(solids[0].Axis.Origin, solids[0].Axis.Direction, solids[1].Axis.Origin, solids[1].Axis.Direction, DoubleTolerance, 
                    out FixedVector<Dim3> centerIfc))
            {
                return;
            }
            
            FixedVector<Dim3> center = centerIfc * lengthPower;
            
            int mainIndex;
            int branchIndex;

            if (!TryClassifySolids(solids, centerIfc, out mainIndex, out branchIndex))
            {
                return;
            }

            SolidGeometry main = solids[mainIndex];
            SolidGeometry branch = solids[branchIndex];
            
            FixedVector<Dim3> mainStart = main.Start * lengthPower;
            FixedVector<Dim3> mainEnd = main.End * lengthPower;
            
            FixedVector<Dim3> branchStart = branch.Start * lengthPower;
            FixedVector<Dim3> branchEnd = branch.End * lengthPower;

            double branchStartDistance = (branchStart - center).L2Norm();
            double branchEndDistance = (branchEnd - center).L2Norm();

            FixedVector<Dim3> branchPortPosition;
            FixedVector<Dim3> branchDirection;

            if (branchStartDistance > branchEndDistance)
            {
                branchPortPosition = branchStart;
                branchDirection = (branchStart - center).Normalize();
            }
            else
            {
                branchPortPosition = branchEnd;
                branchDirection = (branchEnd - center).Normalize();
            }
            
            FixedVector<Dim3> portADirection = (mainStart - center).Normalize();
            FixedVector<Dim3> portBDirection = (mainEnd - center).Normalize();

            double mainDiameter = main.Diameter * lengthPower;
            double branchDiameter = branch.Diameter * lengthPower;

            Tee tee = new Tee(EntityId.New())
            {
                Position = center
            };

            tee.PortA.SetGeometry(
                mainStart,
                portADirection);

            tee.PortB.SetGeometry(
                mainEnd,
                portBDirection);

            tee.PortC.SetGeometry(
                branchPortPosition,
                branchDirection);

            tee.PortA.Metadata.Diameter = mainDiameter;
            tee.PortB.Metadata.Diameter = mainDiameter;
            tee.PortC.Metadata.Diameter = branchDiameter;

            model.Add(tee);
            context.Register(tee, product);
        }

        private static SolidGeometry CreateSolidGeometry(IIfcExtrudedAreaSolid solid, FixedMatrix<Dim4> globalMatrix)
        {
            FixedMatrix<Dim4> matrix = globalMatrix * solid.Position.ToFixedMatrix();
            FixedMatrix<Dim3> rotation = matrix.GetRotation();
            FixedVector<Dim3> origin = matrix.GetTranslation();

            FixedVector<Dim3> direction = (rotation * solid.ExtrudedDirection.ToFixedVector()).Normalize();
            FixedVector<Dim3> end = origin + direction * solid.Depth;

            IIfcCircleProfileDef profile = (IIfcCircleProfileDef)solid.SweptArea;
            return new SolidGeometry(new SolidAxis(origin, direction), origin, end, profile.Radius * 2);
        }

        private static bool TryClassifySolids(
            SolidGeometry[] solids,
            FixedVector<Dim3> center,
            out int mainIndex,
            out int branchIndex)
        {
            mainIndex = -1;
            branchIndex = -1;

            double firstPosition = GetProjectionParameter(center, solids[0].Start, solids[0].End);
            double secondPosition = GetProjectionParameter(center, solids[1].Start, solids[1].End);

            bool firstContainsCenter = IsInsideSegment(firstPosition);
            bool secondContainsCenter = IsInsideSegment(secondPosition);
            
            if (firstContainsCenter && !secondContainsCenter)
            {
                mainIndex = 0;
                branchIndex = 1;
                return true;
            }

            if (secondContainsCenter && !firstContainsCenter)
            {
                mainIndex = 1;
                branchIndex = 0;
                return true;
            }
            
            if (firstContainsCenter && secondContainsCenter)
            {
                double firstDistance = Math.Min((center - solids[0].Start).L2Norm(), (center - solids[0].End).L2Norm());
                double secondDistance = Math.Min((center - solids[1].Start).L2Norm(), (center - solids[1].End).L2Norm());

                if (firstDistance >= secondDistance)
                {
                    mainIndex = 0;
                    branchIndex = 1;
                }
                else
                {
                    mainIndex = 1;
                    branchIndex = 0;
                }

                return true;
            }
            return false;
        }

        private static double GetProjectionParameter(FixedVector<Dim3> point, FixedVector<Dim3> start, FixedVector<Dim3> end)
        {
            FixedVector<Dim3> direction = (end - start).Normalize();
            return (point - start).Dot(direction);
        }

        private static bool IsInsideSegment(double position)
        {
            return position >= -DoubleTolerance;
        }

        private readonly struct SolidGeometry
        {
            public readonly SolidAxis Axis;
            public readonly FixedVector<Dim3> Start;
            public readonly FixedVector<Dim3> End;
            public readonly double Diameter;

            public SolidGeometry(
                SolidAxis axis,
                FixedVector<Dim3> start,
                FixedVector<Dim3> end,
                double diameter)
            {
                Axis = axis;
                Start = start;
                End = end;
                Diameter = diameter;
            }
        }

        private readonly struct SolidAxis
        {
            public readonly FixedVector<Dim3> Origin;
            public readonly FixedVector<Dim3> Direction;

            public SolidAxis(
                FixedVector<Dim3> origin,
                FixedVector<Dim3> direction)
            {
                Origin = origin;
                Direction = direction;
            }
        }

        private static bool TryGetIntersection(
            FixedVector<Dim3> firstOrigin,
            FixedVector<Dim3> firstDirection,
            FixedVector<Dim3> secondOrigin,
            FixedVector<Dim3> secondDirection,
            double tolerance,
            out FixedVector<Dim3> intersection)
        {
            intersection = default(FixedVector<Dim3>);

            FixedVector<Dim3> d1 = firstDirection.Normalize();
            FixedVector<Dim3> d2 = secondDirection.Normalize();
            FixedVector<Dim3> r = firstOrigin - secondOrigin;

            double a = d1.Dot(d1);
            double b = d1.Dot(d2);
            double c = d2.Dot(d2);
            double d = d1.Dot(r);
            double e = d2.Dot(r);

            double denominator = a * c - b * b;
            if (Math.Abs(denominator) <= 1e-12)
                return false;

            double t = (b * e - c * d) / denominator;
            double s = (a * e - b * d) / denominator;

            FixedVector<Dim3> firstPoint = firstOrigin + d1 * t;
            FixedVector<Dim3> secondPoint = secondOrigin + d2 * s;

            double distance = (firstPoint - secondPoint).L2Norm();
            if (distance > tolerance)
                return false;

            intersection = (firstPoint + secondPoint) * 0.5;
            return true;
        }
    }
}