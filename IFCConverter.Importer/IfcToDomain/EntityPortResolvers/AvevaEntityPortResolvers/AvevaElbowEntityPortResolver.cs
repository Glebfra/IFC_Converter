using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.IFC.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.AvevaEntityPortResolvers
{
    internal sealed class AvevaElbowEntityPortResolver : IAvevaEntityPortResolver
    {
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out EntityId id))
                return false;

            Entity entity = model.GetEntity(id);
            return entity is Elbow;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Elbow elbow = (Elbow)model.GetEntity(context.GetEntityId(product));

            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");

            if (!(representationItems[0] is IIfcRevolvedAreaSolid revolvedAreaSolid))
                throw new Exception("The representation item is not a revolved area solid.");

            if (!(revolvedAreaSolid.SweptArea is IIfcCircleProfileDef circleProfileDef))
                throw new Exception("The swept area is not a circle profile.");

            double lengthPower = product.Model.GetLengthPower();
            double circleDiameter = circleProfileDef.Radius * 2 * lengthPower;

            FixedMatrix<Dim4> areaMatrix = revolvedAreaSolid.Position.ToFixedMatrix();

            FixedVector<Dim3> axisLocalPosition = revolvedAreaSolid.Axis.Location.ToFixedVector<Dim3>();
            FixedVector<Dim3> axisGlobalPosition = areaMatrix.GetRotation().LeftMultiply(axisLocalPosition) + areaMatrix.GetTranslation();
            axisGlobalPosition *= lengthPower;

            FixedVector<Dim3> axis = (elbow.Position - axisGlobalPosition).Normalize();
            FixedVector<Dim3> upDirection = axis.CrossProduct(areaMatrix.GetY().ToCartesian()).Normalize();

            FixedMatrix<Dim3>[] rotationMatrices =
            {
                FixedMatrix<Dim3>.Builder.CreateRotationAroundVector(upDirection, revolvedAreaSolid.Angle / 2),
                FixedMatrix<Dim3>.Builder.CreateRotationAroundVector(upDirection, -revolvedAreaSolid.Angle / 2)
            };

            FixedVector<Dim3>[] portPositions = rotationMatrices
                .Select(matrix => matrix.Multiply(axis * elbow.Radius) + axisGlobalPosition)
                .ToArray();
            FixedVector<Dim3>[] portDirections = portPositions.Select(position => (position - elbow.Position).Normalize()).ToArray();

            int i = 0;
            foreach (Port port in elbow.Ports)
            {
                port.SetGeometry(portPositions[i], portDirections[i]);
                port.Metadata.Diameter = circleDiameter;
                i++;
            }
        }
    }
}