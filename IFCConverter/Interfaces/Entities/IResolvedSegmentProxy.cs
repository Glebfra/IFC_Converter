using MathNet.Numerics.LinearAlgebra;
using Start.Interfaces;

namespace IFCConverter.Interfaces
{
    internal interface IResolvedSegmentProxy
    {
        public ISegmentProxy Source { get; }
        
        public Vector<double> ResolvedStartPosition { get; }
        public Vector<double> ResolvedEndPosition { get; }
        
        public Vector<double> ResolvedProjection { get; }

        public IStartSegmentEntity ToStartEntity();
    }
}