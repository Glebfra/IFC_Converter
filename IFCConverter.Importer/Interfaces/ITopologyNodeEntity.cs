using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Interfaces
{
    public interface ITopologyNodeEntity
    {
        public Vector<double> Position { get; }
    }
}