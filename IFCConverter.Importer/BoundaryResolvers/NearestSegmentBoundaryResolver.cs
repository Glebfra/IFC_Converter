using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.BoundaryResolvers
{
    internal sealed class NearestSegmentBoundaryResolver : IBoundaryResolver
    {
        public IReadOnlyCollection<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies, int boundaryCount)
        {
            Vector<double> position = proxy.Position;
            IEnumerable<ISegmentProxy> segmentProxies = allProxies.OfType<ISegmentProxy>();
            return segmentProxies
                .SelectMany(segment => new Vector<double>[]
                {
                    segment.Position, segment.EndPosition
                })
                .OrderBy(segmentPosition => (position - segmentPosition).L2Norm())
                .Take(boundaryCount)
                .ToArray();
        }
    }
}