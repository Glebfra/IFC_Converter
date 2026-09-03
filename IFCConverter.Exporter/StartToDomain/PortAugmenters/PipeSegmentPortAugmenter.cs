using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Extensions;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

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

            Segment entity = (Segment)model.GetEntity(id);

            foreach (IStartFittingEntity startConnectedEntity in source.ConnectedEntities.OfType<IStartFittingEntity>())
            {
                if (!context.TryGetEntityId(startConnectedEntity, out EntityId connectedId))
                    continue;

                Fitting connectedEntity = (Fitting)model.GetEntity(connectedId);

                if (connectedEntity is Reducer reducer)
                {
                    ResolveReducerPort(entity, reducer);
                    continue;
                }
                
                foreach (Vector<double> fittingPos in connectedEntity.Positions)
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

        private static void ResolveReducerPort(Segment segment, Reducer reducer)
        {
            Vector<double> reducerDirection = (reducer.PortB.Position - reducer.PortA.Position).Normalize(2);
            Vector<double> segmentDirection = segment.GetDirectionFromPoint(reducer.Position);

            if (reducerDirection.DotProduct(segmentDirection) < 0)
                return;

            Port segmentNearestPort = segment.GetNearestPort(reducer.PortA);
            segmentNearestPort.SetGeometry(reducer.PortB.Position, reducer.PortB.Direction.Negate());
        }
    }
}