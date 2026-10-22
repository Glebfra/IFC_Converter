using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;

namespace IFCConverter.Domain.Entities
{
    public class Beam : AbstractSegment<BeamMetadata>
    {
        public Beam(Id<EntityId> id) : base(id)
        {
        }

        public double Width { get; set; }
        public double Height { get; set; }
        public double Diameter { get; set; }
        public double SectionAxisAngle { get; set; }

        public override BeamMetadata TypedMetadata { get; } = new BeamMetadata();
    }
}