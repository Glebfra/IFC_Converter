using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public sealed class Valve : Fitting
    {
        public Port PortA { get; }
        public Port PortB { get; }

        public Vector<double> Position { get; set; }
        public double Length { get; set; }

        public Valve(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
        }
    }
}