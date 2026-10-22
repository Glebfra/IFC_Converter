using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain.Entities
{
    public sealed class Elbow : AbstractFitting<ElbowMetadata>
    {
        public Elbow(Id<EntityId> id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
        }

        public Port PortA { get; }
        public Port PortB { get; }

        public double Radius { get; set; }

        public override ElbowMetadata TypedMetadata { get; } = new ElbowMetadata();
    }
}