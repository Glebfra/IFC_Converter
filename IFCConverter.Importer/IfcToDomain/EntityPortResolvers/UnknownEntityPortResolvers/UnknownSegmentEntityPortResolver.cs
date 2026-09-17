using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.UnknownEntityPortResolvers
{
    internal sealed class UnknownSegmentEntityPortResolver : IUnknownEntityPortResolver
    {
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return model.GetEntity(context.GetEntityId(product)) is Segment;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Segment segment = (Segment)model.GetEntity(context.GetEntityId(product));

            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");
            if (!(representationItems[0] is IfcExtrudedAreaSolid extrudedAreaSolid))
                throw new Exception("The representation item is not a extruded area solid.");
            if (!(product.ObjectPlacement is IIfcLocalPlacement localPlacement))
                throw new Exception("The given product is not a local placement.");

            FixedMatrix<Dim4> globalMatrix = extrudedAreaSolid.Position.ToFixedMatrix();

            while (localPlacement != null)
            {
                FixedMatrix<Dim4> localMatrix = localPlacement.RelativePlacement.ToFixedMatrix();
                globalMatrix = localMatrix * globalMatrix;
                localPlacement = localPlacement.PlacementRelTo as IIfcLocalPlacement;
            }

            FixedVector<Dim3> position = globalMatrix.GetTranslation();
            FixedMatrix<Dim3> rotation = globalMatrix.GetRotation();

            FixedVector<Dim3> direction = rotation * extrudedAreaSolid.ExtrudedDirection.ToFixedVector();
            double length = extrudedAreaSolid.Depth;

            double lengthPower = product.Model.GetLengthPower();
            FixedVector<Dim3> startPos = position * lengthPower;
            FixedVector<Dim3> endPos = startPos + direction * (lengthPower * length);

            segment.StartPort.SetGeometry(startPos, direction);
            segment.EndPort.SetGeometry(endPos, direction.Negate());

            segment.StartPort.Metadata.Diameter = segment.Diameter;
            segment.EndPort.Metadata.Diameter = segment.Diameter;
        }
    }
}