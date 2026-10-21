using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain.Entities
{
    public class Equipment : AbstractFitting<EquipmentMetadata>
    {

        public Equipment(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
        }

        public Port PortA { get; set; }
        public Port PortB { get; set; }

        public override EquipmentMetadata TypedMetadata { get; } = new EquipmentMetadata();
    }
}