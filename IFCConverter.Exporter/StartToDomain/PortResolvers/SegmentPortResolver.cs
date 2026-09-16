using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class SegmentPortResolver : IPortResolver
    {
        public bool CanResolve(IStartEntity source)
        {
            return source is StartAbstractSegmentEntity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out EntityId id))
                return;
            StartAbstractSegmentEntity start = (StartAbstractSegmentEntity)source;
            AbstractSegment entity = (AbstractSegment)model.GetEntity(id);

            ResolvePortA(start, entity);
            ResolvePortB(start, entity);
        }

        private static void ResolvePortA(IStartSegmentEntity startSegment, AbstractSegment segment)
        {
            FixedVector<Dim3> position = startSegment.StartNode.Position;
            FixedVector<Dim3> direction = ResolveDirection(startSegment, position);

            segment.StartPort.SetGeometry(position, direction);
            segment.StartPort.Metadata.Diameter = startSegment.Diameter.SIProperty;
        }

        private static void ResolvePortB(IStartSegmentEntity startSegment, AbstractSegment segment)
        {
            FixedVector<Dim3> position = startSegment.EndNode.Position;
            FixedVector<Dim3> direction = ResolveDirection(startSegment, position);

            segment.EndPort.SetGeometry(position, direction);

            switch (startSegment)
            {
                case StartConeElementEntity coneElementEntity:
                    segment.EndPort.Metadata.Diameter = coneElementEntity.SecondDiameter.SIProperty;
                    break;
                default:
                    segment.EndPort.Metadata.Diameter = startSegment.Diameter.SIProperty;
                    break;
            }
        }

        private static FixedVector<Dim3> ResolveDirection(IStartSegmentEntity startSegment, FixedVector<Dim3> position)
        {
            return startSegment.GetProjectionFromPoint(position).Normalize().Negate();
        }
    }
}