using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain.Entities
{
    public sealed class PipeSegment : Entity
    {
        public Port StartPort { get; }
        public Port EndPort { get; }
        
        public double? Diameter { get; set; }
        public double? WallThickness { get; set; }
        
        public PipeSegment(EntityId id) : base(id)
        {
            StartPort = CreatePort();
            EndPort = CreatePort();
        }
    }
}