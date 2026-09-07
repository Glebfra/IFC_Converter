using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public sealed class Joint : AbstractFitting
    {
        public Port PortA { get; }
        public Port PortB { get; }
        
        public double Length { get; set; }
        
        public Joint(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
        }
    }
}