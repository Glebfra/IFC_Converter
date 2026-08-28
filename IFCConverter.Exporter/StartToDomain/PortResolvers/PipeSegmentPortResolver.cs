using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class PipeSegmentPortResolver : IPortResolver
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
            PipeSegment entity = (PipeSegment)model.GetEntity(id);

            ResolvePortA(start, entity);
            ResolvePortB(start, entity);
        }

        private static void ResolvePortA(IStartSegmentEntity startSegment, PipeSegment segment)
        {
            Vector<double> position = startSegment.StartNode.Position;
            Vector<double> direction = ResolveDirection(startSegment, position);

            segment.StartPort.SetGeometry(position, direction);
            segment.StartPort.Metadata.Diameter = startSegment.Diameter.SIProperty;
        }

        private static void ResolvePortB(IStartSegmentEntity startSegment, PipeSegment segment)
        {
            Vector<double> position = startSegment.EndNode.Position;
            Vector<double> direction = ResolveDirection(startSegment, position);

            segment.EndPort.SetGeometry(position, direction);
            segment.EndPort.Metadata.Diameter = startSegment.Diameter.SIProperty;
        }

        private static Vector<double> ResolveDirection(IStartSegmentEntity startSegment, Vector<double> position)
        {
            return startSegment.GetProjectionFromPoint(position).Normalize(2).Negate();
        }
    }
}