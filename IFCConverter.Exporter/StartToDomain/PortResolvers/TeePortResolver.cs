using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class TeePortResolver : IPortResolver
    {

        public bool CanResolve(IStartEntity source)
        {
            return source is StartAbstractTeeEntity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out EntityId id))
                return;

            StartAbstractTeeEntity start = (StartAbstractTeeEntity)source;
            Tee entity = (Tee)model.GetEntity(id);

            IStartSegmentEntity[] segments = start.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            if (segments.Length != 3)
                throw new InvalidOperationException($"Tee '{entity.Id}' must have exactly three connected segments");

            FilteredSegments filteredSegments = FilterSegments(segments);
            ResolvePort(filteredSegments.MainSegments[0], entity.PortA, entity.Position, start.MainLength / 2, model, context);
            ResolvePort(filteredSegments.MainSegments[1], entity.PortB, entity.Position, start.MainLength / 2, model, context);
            ResolvePort(filteredSegments.HeadSegment, entity.PortC, entity.Position, start.HeadLength, model, context);
        }

        private static FilteredSegments FilterSegments(IStartSegmentEntity[] segmentEntities)
        {
            IStartSegmentEntity[] mainSegments = new IStartSegmentEntity[2];
            IStartSegmentEntity headSegment = null;

            for (int i = 0; i < 3; i++)
            for (int j = i + 1; j < 3; j++)
            {
                if (!segmentEntities[i].Projection.IsParallel(segmentEntities[j].Projection))
                    continue;

                mainSegments[0] = segmentEntities[i];
                mainSegments[1] = segmentEntities[j];
                headSegment = segmentEntities[3 - (i + j)];
            }

            if (headSegment == null)
                throw new InvalidOperationException("Cannot filter segments on tee");

            return new FilteredSegments
            {
                HeadSegment = headSegment,
                MainSegments = mainSegments
            };
        }

        private static void ResolvePort(IStartSegmentEntity startSegment, Port port, Vector<double> position, double length, EngineeringModel model, StartMappingContext context)
        {
            Vector<double> direction = startSegment.GetProjectionFromPoint(position).Normalize(2);
            port.SetGeometry(position + direction * length, direction);
            port.Metadata.Diameter = DiameterFinder.GetDiameter(startSegment, model, context);
        }

        private struct FilteredSegments
        {
            public IStartSegmentEntity HeadSegment;
            public IStartSegmentEntity[] MainSegments;
        }
    }
}