using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class AnchorPortResolver : IPortResolver
    {
        public bool CanResolve(IStartEntity source)
        {
            return source is StartAbstractAnchorEntity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Anchor anchor = (Anchor)model.GetEntity(context.GetEntityId(source));

            IStartSegmentEntity[] segments = source.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            double diameter = DiameterFinder.GetMaxDiameter(segments, model, context);

            FixedVector<Dim3> position = anchor.Position;
            FixedVector<Dim3> direction = CalculateDirection(source, segments, position);

            anchor.Port.SetGeometry(position, direction);
            anchor.Port.Metadata.Diameter = diameter;
        }

        private static FixedVector<Dim3> CalculateDirection(IStartEntity source, IStartSegmentEntity[] segments, FixedVector<Dim3> position)
        {
            switch (source)
            {
                case StartFixedAnchorEntity _:
                    return segments.First().GetProjectionFromPoint(position).Normalize();
                case StartAbstractAnchorEntity _:
                    return FixedVector<Dim3>.Builder.Z();
            }

            throw new InvalidOperationException($"Cannot calculate direction for {source}");
        }
    }
}