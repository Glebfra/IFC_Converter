using IFCConverter.Domain.Identity;

namespace IFCConverter.Domain.Entities
{
    public class Segment : AbstractSegment
    {
        public double Diameter { get; set; }
        
        public Segment(EntityId id) : base(id)
        {
        }
    }
}