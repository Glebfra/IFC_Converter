using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.Entities.Topologies
{
    internal sealed class NodeTopologyResolver : INodeTopologyResolver
    {
        private readonly VectorComparer _comparer;

        public NodeTopologyResolver(VectorComparer comparer)
        {
            _comparer = comparer;
        }

        public IReadOnlyCollection<ITopologyNodeEntity> ResolveTopology(
            IEntityProxy proxy,
            IReadOnlyCollection<IEntityProxy> connectedProxies)
        {
            List<ITopologyNodeEntity> nodes = new();
            if (proxy is IFittingProxy fittingProxy)
            {
                nodes.Add(new TopologyNode(fittingProxy.Position));
            }
            else if (proxy is ISegmentProxy segmentProxy)
            {
                nodes.Add(ResolveSegmentNode(segmentProxy.Position, connectedProxies));
                nodes.Add(ResolveSegmentNode(segmentProxy.EndPosition, connectedProxies));
            }

            return nodes;
        }

        private TopologyNode ResolveSegmentNode(
            Vector<double> segmentPoint,
            IReadOnlyCollection<IEntityProxy> connectedProxies)
        {
            #if true

            foreach (IEntityProxy connectedProxy in connectedProxies)
            {
                bool isConnectedToPoint = connectedProxy.Boundary
                    .Any(bound => _comparer.Equals(bound, segmentPoint));
                if (!isConnectedToPoint)
                    continue;

                return new TopologyNode(connectedProxy.Position);
            }

            return new TopologyNode(segmentPoint);

            #else
            IFittingProxy? fitting = connectedProxies
                .OfType<IFittingProxy>()
                .OrderBy(proxy => (proxy.Position - segmentPoint).L2Norm())
                .FirstOrDefault();
            return new TopologyNode(fitting?.Position ?? segmentPoint);

            #endif
        }
    }
}