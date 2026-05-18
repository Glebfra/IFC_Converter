using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Interfaces
{
    internal interface ISegmentProxy : IEntityProxy
    {
        public double Length { get; }
        public Vector<double> Position { get; }
        public Vector<double> Direction { get; }
    }
}