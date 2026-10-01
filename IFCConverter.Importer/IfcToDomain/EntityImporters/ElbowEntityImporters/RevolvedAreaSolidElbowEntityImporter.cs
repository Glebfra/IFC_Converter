using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ElbowEntityImporters
{
    internal sealed class RevolvedAreaSolidElbowEntityImporter : IElbowEntityImporter
    {
        public bool CanImport(IIfcProduct product)
        {
            if (!(product.ObjectPlacement is IIfcLocalPlacement localPlacement))
                return false;
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                return false;
            if (!(representationItems[0] is IIfcRevolvedAreaSolid revolvedAreaSolid))
                return false;
            if (!(revolvedAreaSolid.SweptArea is IIfcCircleProfileDef circleProfileDef))
                return false;

            return true;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            IIfcLocalPlacement localPlacement = (IIfcLocalPlacement)product.ObjectPlacement;
            IIfcRevolvedAreaSolid revolvedAreaSolid = (IIfcRevolvedAreaSolid)product.GetRepresentationItems().First();
            IIfcCircleProfileDef circleProfileDef = (IIfcCircleProfileDef)revolvedAreaSolid.SweptArea;

            double lengthPower = product.Model.GetLengthPower();
            double angle = revolvedAreaSolid.Angle;
            double circleDiameter = circleProfileDef.Radius * 2 * lengthPower;

            FixedMatrix<Dim4> globalMatrix = localPlacement.GetGlobalMatrix();
            globalMatrix = globalMatrix * revolvedAreaSolid.Position.ToFixedMatrix();

            FixedVector<Dim3> globalPosition = globalMatrix.GetTranslation() * lengthPower;
            FixedMatrix<Dim3> globalRotation = globalMatrix.GetRotation();
            
            FixedVector<Dim3> axisDirection = globalRotation * revolvedAreaSolid.Axis.Axis.ToFixedVector();
            FixedVector<Dim3> axisPosition = globalPosition + globalRotation * revolvedAreaSolid.Axis.Location.ToFixedVector<Dim3>() * lengthPower;

            double radius = (axisPosition - globalPosition).L2Norm();
            double displacement = CalculateDisplacement(radius, angle);

            FixedMatrix<Dim3> angleRotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundVector(axisDirection, angle / 2);
            FixedVector<Dim3> OA = globalPosition - axisPosition;
            FixedVector<Dim3> OC = (angleRotation * OA).Normalize() * displacement;

            FixedVector<Dim3> center = axisPosition + OC;

            Elbow elbow = new Elbow(EntityId.New())
            {
                Radius = radius,
                Position = center
            };

            FixedVector<Dim3> localAxisPosition = revolvedAreaSolid.Axis.Location.ToFixedVector<Dim3>() * lengthPower;
            FixedVector<Dim3> globalAxisPosition = globalRotation * localAxisPosition + globalPosition;
            
            FixedVector<Dim3> localAxisDirection = revolvedAreaSolid.Axis.Axis.ToFixedVector();
            FixedMatrix<Dim3> localAngleRotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundVector(localAxisDirection, revolvedAreaSolid.Angle);

            FixedVector<Dim3> localFirstPosition = localAxisPosition.Negate();
            FixedVector<Dim3> localSecondPosition = localAngleRotation * localFirstPosition;

            FixedVector<Dim3>[] portPositions = new FixedVector<Dim3>[]
            {
                globalPosition,
                globalRotation * localSecondPosition + globalAxisPosition
            };
            FixedVector<Dim3>[] portDirections = portPositions.Select(position => (position - center).Normalize()).ToArray();

            int i = 0;
            foreach (Port port in elbow.Ports)
            {
                port.SetGeometry(portPositions[i], portDirections[i]);
                port.Metadata.Diameter = circleDiameter;
                i++;
            }
            
            model.Add(elbow);
            context.Register(elbow, product);
        }
        
        private static double CalculateDisplacement(double radius, double angle)
        {
            return radius / Math.Cos(angle / 2);
        }
    }
}