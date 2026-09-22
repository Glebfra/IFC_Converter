using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.UnknownEntityPortResolvers
{
    internal sealed class UnknownTeePortResolver : IUnknownEntityPortResolver
    {
        private const double DoubleTolerance = 1e-6;
        private readonly VectorComparer _comparer = new VectorComparer(DoubleTolerance);
        
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out EntityId id))
                return false;
            Entity entity = model.GetEntity(id);

            return entity is Tee;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Tee tee = (Tee)model.GetEntity(context.GetEntityId(product));
            
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 2)
                throw new Exception("Expected exactly two representation item for the given source.");
            
            if (!representationItems.Any(item => item is IIfcExtrudedAreaSolid))
                throw new Exception("The representation items is not a extruded area solid.");
            IIfcExtrudedAreaSolid[] extrudedAreaSolids = representationItems.Cast<IIfcExtrudedAreaSolid>().ToArray();

            FixedVector<Dim3> position = tee.Position;
            FixedMatrix<Dim4> globalMatrix = (FixedMatrix<Dim4>)tee.Metadata.Meta["GlobalMatrix"];
            
            double lengthPower = product.Model.GetLengthPower();
            FixedVector<Dim3> mainProjection = default, headProjection = default;
            double mainDiameter = default, headDiameter = default;
            
            foreach (IIfcExtrudedAreaSolid extrudedAreaSolid in extrudedAreaSolids)
            {
                if (!(extrudedAreaSolid.SweptArea is IIfcCircleProfileDef profileDef))
                    throw new Exception("The swept area is not a circle profile definition.");

                double teeBranchDiameter = profileDef.Radius * 2 * lengthPower;

                FixedMatrix<Dim4> matrix = globalMatrix * extrudedAreaSolid.Position.ToFixedMatrix();
                FixedMatrix<Dim3> rotation = matrix.GetRotation();

                FixedVector<Dim3> extrudedDir = extrudedAreaSolid.ExtrudedDirection.ToFixedVector();
                FixedVector<Dim3> teeBranchDir = (rotation * extrudedDir).Normalize();
                double teeBranchLength = extrudedAreaSolid.Depth * lengthPower;

                FixedVector<Dim3> startPos = matrix.GetTranslation() * lengthPower;
                FixedVector<Dim3> projection = teeBranchDir * teeBranchLength;
                FixedVector<Dim3> endPos = startPos + projection;

                if (_comparer.Equals(startPos, position))
                {
                    headProjection = projection;
                    headDiameter = teeBranchDiameter;
                }
                else if (_comparer.Equals(endPos, position))
                {
                    headProjection = projection.Negate();
                    headDiameter = teeBranchDiameter;
                }
                else
                {
                    mainProjection = projection;
                    mainDiameter = teeBranchDiameter;
                }
            }

            FixedVector<Dim3> mainDirection = mainProjection.Normalize();
            FixedVector<Dim3> headDirection = headProjection.Normalize();

            tee.PortA.SetGeometry(position - mainProjection * (1.0 / 2), mainDirection.Negate());
            tee.PortB.SetGeometry(position + mainProjection * (1.0 / 2), mainDirection);
            tee.PortC.SetGeometry(position + headProjection, headDirection);

            tee.PortA.Metadata.Diameter = mainDiameter;
            tee.PortB.Metadata.Diameter = mainDiameter;
            tee.PortC.Metadata.Diameter = headDiameter;
        }
    }
}