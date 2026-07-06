using MathNet.Numerics.LinearAlgebra;
using Start.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IResolvedSegmentProxy : IResolvedProxy
    {
        public Vector<double> ResolvedStartPosition { get; }
        public Vector<double> ResolvedEndPosition { get; }

        public Vector<double> ResolvedProjection { get; }

        public IStartSegmentEntity ToStartEntity();
    }
}