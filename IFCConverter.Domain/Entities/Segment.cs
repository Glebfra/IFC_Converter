using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain.Entities
{
    public class Segment : Entity
    {
        public Segment(EntityId id) : base(id)
        {
            StartPort = CreatePort();
            EndPort = CreatePort();
        }

        public Port StartPort { get; }
        public Port EndPort { get; }

        public double Diameter { get; set; }
    }
}