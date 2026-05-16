using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Interfaces
{
    internal interface IFittingProxy : IEntityProxy, ITopologyProxy
    {
        public Vector<double> Position { get; }
    }
}