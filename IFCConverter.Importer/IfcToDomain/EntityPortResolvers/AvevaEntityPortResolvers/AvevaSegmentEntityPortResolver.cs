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
    internal sealed class AvevaSegmentEntityPortResolver : IAvevaEntityPortResolver
    {
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out EntityId id))
                return false;

            Entity entity = model.GetEntity(id);
            return entity is Segment;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Segment segment = (Segment)model.GetEntity(context.GetEntityId(product));

            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");

            if (!(representationItems[0] is IIfcExtrudedAreaSolid extrudedAreaSolid))
                throw new Exception("The representation item is not an extruded area solid.");

            FixedMatrix<Dim4> position = extrudedAreaSolid.Position.ToFixedMatrix();
            FixedMatrix<Dim3> rotation = position.GetRotation();
            FixedVector<Dim3> extrudedDirection = extrudedAreaSolid.ExtrudedDirection.ToFixedVector();
            FixedVector<Dim3> pipeDirection = rotation.LeftMultiply(extrudedDirection).Normalize();

            double lengthPower = product.Model.GetLengthPower();
            double length = extrudedAreaSolid.Depth * lengthPower;

            FixedVector<Dim3> startPos = position.GetTranslation() * lengthPower;
            FixedVector<Dim3> endPos = startPos + length * pipeDirection;

            FixedVector<Dim3>[] positions =
            {
                startPos, endPos
            };
            FixedVector<Dim3>[] directions =
            {
                pipeDirection.Negate(), pipeDirection
            };

            int i = 0;
            foreach (Port port in segment.Ports)
            {
                port.SetGeometry(positions[i], directions[i]);
                port.Metadata.Diameter = segment.Diameter;
                i++;
            }
        }
    }
}