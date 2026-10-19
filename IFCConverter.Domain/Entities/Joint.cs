using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain.Entities
{
    public sealed class Joint : AbstractFitting<JointMetadata>
    {

        public Joint(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
        }

        public Port PortA { get; }
        public Port PortB { get; }

        public double Length { get; set; }

        public override JointMetadata TypedMetadata { get; } = new JointMetadata();
    }
}