using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Interfaces
{
    internal interface IFittingProxy : IEntityProxy
    {
        public Vector<double> Position { get; }
    }
}