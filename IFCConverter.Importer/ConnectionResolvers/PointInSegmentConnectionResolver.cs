using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.ConnectionResolvers
{
    internal sealed class PointInSegmentConnectionResolver : IConnectionResolver
    {
        public IEnumerable<IBoundaryProxy> GetConnectedEntities(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> allProxies)
        {
            IEnumerable<IBoundaryProxy> segmentProxies = allProxies
                .Where(boundaryProxy => boundaryProxy.Proxy is ISegmentProxy);

            Vector<double> position = proxy.Proxy.Position;
            return segmentProxies.Where(segment => IsSegmentContainPoint(position, (ISegmentProxy)segment.Proxy));
        }

        private static bool IsSegmentContainPoint(Vector<double> point, ISegmentProxy segmentProxy)
        {
            Vector<double> direction = segmentProxy.Direction.Normalize(2);
            Vector<double> startPos = segmentProxy.Position;
            Vector<double> endPos = segmentProxy.EndPosition;

            Vector<double> toPoint = point - startPos;
            double pointProjection = toPoint.DotProduct(direction);

            if (pointProjection < 0 || pointProjection > segmentProxy.Length)
                return false;

            Vector<double> closestPointOnAxis = startPos + direction * pointProjection;
            double radialDistance = (point - closestPointOnAxis).L2Norm();
            double radius = segmentProxy.Diameter / 2;
            return radialDistance <= radius;
        }
    }
}