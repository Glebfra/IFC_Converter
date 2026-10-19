using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain.Entities
{
    public sealed class Reducer : AbstractFitting<ReducerMetadata>
    {
        public Reducer(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
        }

        public Port PortA { get; }
        public Port PortB { get; }

        public double Length { get; set; }

        public override ReducerMetadata TypedMetadata { get; } = new ReducerMetadata();
    }
}