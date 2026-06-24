using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Interfaces;

namespace IFCConverter.Importer.Proxies
{
    internal sealed class ResolvedSegmentProxy : IResolvedSegmentProxy
    {
        private ResolvedSegmentProxy(
            ISegmentProxy source,
            Vector<double> resolvedStartPosition,
            Vector<double> resolvedEndPosition,
            Vector<double> projection)
        {
            Source = source;
            ResolvedStartPosition = resolvedStartPosition;
            ResolvedEndPosition = resolvedEndPosition;
            ResolvedProjection = projection;
        }

        public ISegmentProxy Source { get; }
        public Vector<double> ResolvedStartPosition { get; }
        public Vector<double> ResolvedEndPosition { get; }
        public Vector<double> ResolvedProjection { get; }

        public IStartSegmentEntity ToStartEntity()
        {
            IStartSegmentEntity startEntity = (IStartSegmentEntity)Source.ToStartEntity();
            startEntity.StartPosition = ResolvedStartPosition;
            startEntity.Projection = ResolvedProjection;

            return startEntity;
        }

        public static ResolvedSegmentProxy CreateFromSegmentProxy(
            ISegmentProxy segmentProxy,
            Vector<double> resolvedStartPosition,
            Vector<double> resolvedEndPosition)
        {
            Vector<double> segmentProjection = segmentProxy.Direction * segmentProxy.Length;
            Vector<double> resolvedProjection = resolvedEndPosition - resolvedStartPosition;

            Vector<double> realProjection = segmentProjection.Normalize(2) * resolvedProjection.L2Norm();

            return new ResolvedSegmentProxy(segmentProxy, resolvedStartPosition, resolvedEndPosition, realProjection);
        }
    }
}