using IFCConverter.Domain.Identity;

namespace IFCConverter.Domain.Entities
{
    public class Beam : AbstractSegment
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Diameter { get; set; }
        public double SectionAxisAngle { get; set; }
        
        public Beam(EntityId id) : base(id)
        {
        }
    }
}