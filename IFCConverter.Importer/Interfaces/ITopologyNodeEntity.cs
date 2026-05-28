using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologyNodeEntity
    {
        public Vector<double> Position { get; }
    }
}