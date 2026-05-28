using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Interfaces
{
    public interface ISegmentProxy : IEntityProxy
    {
        public double Length { get; }
        public Vector<double> Direction { get; }
        public Vector<double> EndPosition { get; }
    }
}