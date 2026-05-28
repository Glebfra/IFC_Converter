using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Interfaces
{
    internal interface ITopologyNodeEntity
    {
        public Vector<double> Position { get; }
    }
}