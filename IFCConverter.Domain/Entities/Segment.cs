using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;

namespace IFCConverter.Domain.Entities
{
    public class Segment : AbstractSegment<SegmentMetadata>
    {

        public Segment(EntityId id) : base(id)
        {
        }

        public double Diameter { get; set; }

        public override SegmentMetadata TypedMetadata { get; } = new SegmentMetadata();
    }
}