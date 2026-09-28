using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.SegmentEntityPortResolvers
{
    internal sealed class ExtrudedAreaSolidSegmentEntityPortResolver : ISegmentEntityPortResolver
    {
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                return false;
            if (!(representationItems[0] is IfcExtrudedAreaSolid extrudedAreaSolid))
                return false;
            if (!(product.ObjectPlacement is IIfcLocalPlacement localPlacement))
                return false;

            return true;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Segment segment = (Segment)model.GetEntity(context.GetEntityId(product));
            
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            IfcExtrudedAreaSolid extrudedAreaSolid = (IfcExtrudedAreaSolid)representationItems[0];
            IIfcLocalPlacement localPlacement = (IIfcLocalPlacement)product.ObjectPlacement;
            
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