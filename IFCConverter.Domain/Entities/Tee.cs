using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain.Entities
{
    public sealed class Tee : AbstractFitting<TeeMetadata>
    {

        public Tee(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
            PortC = CreatePort();
        }

        public Port PortA { get; }
        public Port PortB { get; }
        public Port PortC { get; }

        public override TeeMetadata TypedMetadata { get; } = new TeeMetadata();
    }
}