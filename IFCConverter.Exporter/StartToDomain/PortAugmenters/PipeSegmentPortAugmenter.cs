using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Extensions;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Exporter.StartToDomain.PortAugmenters
{
    internal sealed class PipeSegmentPortAugmenter : IPortAugmenter
    {
        private const double Tolerance = 1e-6;

        public bool CanAugment(IStartEntity source)
        {
            return source is StartAbstractSegmentEntity;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out EntityId id))
                return;

            AbstractSegment entity = (AbstractSegment)model.GetEntity(id);

            foreach (IStartFittingEntity startConnectedEntity in source.ConnectedEntities.OfType<IStartFittingEntity>())
            {
                if (!context.TryGetEntityId(startConnectedEntity, out EntityId connectedId))
                    continue;

                AbstractFitting connectedEntity = (AbstractFitting)model.GetEntity(connectedId);

                if (connectedEntity is Reducer reducer)
                {
                    ResolveReducerPort(entity, reducer);
                    continue;
                }

                foreach (FixedVector<Dim3> fittingPos in connectedEntity.Positions)
                {
                    if (!entity.IsSegmentContainPoint(fittingPos))
                        continue;

                    foreach (Port connectedEntityPort in connectedEntity.Ports)
                    {
                        if (!entity.IsSegmentContainPoint(connectedEntityPort.Position))
                            continue;

                        Port entityPort = entity.GetNearestPort(fittingPos);
                        entityPort.SetGeometry(connectedEntityPort.Position, connectedEntityPort.Direction.Negate());
                    }
                }
            }
        }

        private static void ResolveReducerPort(AbstractSegment segment, Reducer reducer)
        {
            FixedVector<Dim3> reducerDirection = (reducer.PortB.Position - reducer.PortA.Position).Normalize();
            FixedVector<Dim3> segmentDirection = segment.GetDirectionFromPoint(reducer.Position);

            if (reducerDirection.Dot(segmentDirection) < 0)
                return;

            Port segmentNearestPort = segment.GetNearestPort(reducer.PortA);
            segmentNearestPort.SetGeometry(reducer.PortB.Position, reducer.PortB.Direction.Negate());
        }
    }
}