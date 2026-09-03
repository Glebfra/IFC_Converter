using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain.Entities
{
    public class Equipment : Fitting
    {
        public Port PortA { get; set; }
        public Port PortB { get; set; }
        
        public Equipment(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
        }
    }
}