using IFCConverter.Domain.Identity;

namespace IFCConverter.Domain.Entities
{
    public class Segment : AbstractSegment
    {

        public Segment(EntityId id) : base(id)
        {
        }

        public double Diameter { get; set; }
    }
}