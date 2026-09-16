using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain.Entities
{
    public class Equipment : AbstractFitting
    {

        public Equipment(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
        }

        public Port PortA { get; set; }
        public Port PortB { get; set; }
    }
}