using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Extensions
{
    public static class SegmentCreator
    {
        public static Segment CreateSegment(
            FixedVector<Dim3> start, FixedVector<Dim3> end, 
            double diameter, string name = null,
            FixedVector<Dim3> projection = null)
        {
            FixedVector<Dim3> direction = (end - start).SafeNormalize();

            Segment segment = new Segment(Id<EntityId>.New())
            {
                Diameter = diameter
            };

            FixedVector<Dim3>[] positions = new FixedVector<Dim3>[] { start, end };
            FixedVector<Dim3>[] directions = new FixedVector<Dim3>[] { direction, direction.Negate() };
            
            segment.StartPort.SetGeometry(start, direction);
            segment.EndPort.SetGeometry(end, direction.Negate());

            int i = 0;
            foreach (Port segmentPort in segment.Ports)
            {
                segmentPort.Metadata.Diameter = diameter;
                segmentPort.SetGeometry(positions[i], directions[i++]);
            }

            segment.Metadata.Name = name ?? string.Empty;

            if (projection != null)
                segment.TypedMetadata.Projection = projection;
            
            return segment;
        }
    }
}