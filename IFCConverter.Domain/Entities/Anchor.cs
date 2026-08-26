using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public sealed class Anchor : Fitting
    {
        public Port Port { get; }
        public Vector<double> Position { get; set; }
        
        public Anchor(EntityId id) : base(id)
        {
            Port = CreatePort();
        }
    }
}