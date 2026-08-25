using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public sealed class Tee : Fitting
    {
        public Port PortA { get; }
        public Port PortB { get; }
        public Port PortC { get; }
        
        public Vector<double> Position { get; set; }

        public Tee(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
            PortC = CreatePort();
        }
    }
}