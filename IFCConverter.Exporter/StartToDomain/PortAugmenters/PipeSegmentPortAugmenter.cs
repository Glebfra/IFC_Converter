using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Extensions;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Segments;
using Start.Interfaces;
using Utils;

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
            
            PipeSegment entity = (PipeSegment)model.GetEntity(id);
            
            foreach (IStartFittingEntity startConnectedEntity in source.ConnectedEntities.OfType<IStartFittingEntity>())
            {
                if (!context.TryGetEntityId(startConnectedEntity, out EntityId connectedId))
                    continue;
                
                Entity connectedEntity = model.GetEntity(connectedId);
                
                if (connectedEntity is Reducer reducer)
                {
                    ResolveReducerPort(entity, reducer);
                    continue;
                }
                
                foreach (Port connectedEntityPort in connectedEntity.Ports)
                {
                    if (!IsSegmentContainPoint(entity, connectedEntityPort.Position))
                        continue;
                    
                    Port entityPort = entity.StartPort.Position.IsNearerThan(entity.EndPort.Position, connectedEntityPort.Position)
                        ? entity.StartPort
                        : entity.EndPort;
                    entityPort.SetGeometry(connectedEntityPort.Position, connectedEntityPort.Direction.Negate());
                }
            }
        }

        private bool IsSegmentContainPoint(PipeSegment segment, Vector<double> point)
        {
            const double epsilon = 1e-6;

            Vector<double> start = segment.StartPort.Position;
            Vector<double> end = segment.EndPort.Position;

            Vector<double> direction = end - start;
            Vector<double> toPoint = point - start;

            double lengthSquared = direction.DotProduct(direction);

            if (lengthSquared < epsilon * epsilon)
                return (point - start).L2Norm() < epsilon;

            double t = toPoint.DotProduct(direction) / lengthSquared;
            if (t < -epsilon || t > 1.0 + epsilon)
                return false;

            Vector<double> projection = start + direction * t;
            return (point - projection).L2Norm() < epsilon;
        }

        private static void ResolveReducerPort(PipeSegment segment, Reducer reducer)
        {
            Vector<double> reducerDirection = (reducer.PortB.Position - reducer.PortA.Position).Normalize(2);
            Vector<double> segmentDirection = segment.GetDirectionFromPoint(reducer.Position);

            if (!reducerDirection.AlmostEqual(segmentDirection, Tolerance))
                return;
        }
    }
}