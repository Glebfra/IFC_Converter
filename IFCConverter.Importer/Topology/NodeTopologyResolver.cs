using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.Topology
{
    internal sealed class NodeTopologyResolver : INodeTopologyResolver
    {
        private readonly VectorComparer _comparer;

        public NodeTopologyResolver()
        {
            _comparer = new VectorComparer(1e-3);
        }

        public NodeTopologyResolver(VectorComparer comparer)
        {
            _comparer = comparer;
        }

        [Pure]
        public IEnumerable<ITopologyNodeEntity> ResolveTopologyRaw(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> connected)
        {
            IEnumerable<IBoundaryProxy> connectedProxies = connected as IBoundaryProxy[] ?? connected.ToArray();

            List<ITopologyNodeEntity> nodes = new List<ITopologyNodeEntity>();
            if (proxy.Proxy is IFittingProxy fittingProxy)
                nodes.Add(new TopologyNode(fittingProxy.Position));
            else if (proxy.Proxy is ISegmentProxy segmentProxy)
            {
                nodes.Add(new TopologyNode(segmentProxy.Position));
                nodes.Add(new TopologyNode(segmentProxy.EndPosition));
            }

            return nodes;
        }

        [Pure]
        public IEnumerable<ITopologyNodeEntity> ResolveTopology(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> connected)
        {
            IEnumerable<IBoundaryProxy> connectedProxies = connected as IBoundaryProxy[] ?? connected.ToArray();

            List<ITopologyNodeEntity> nodes = new();
            if (proxy.Proxy is IFittingProxy fittingProxy)
            {
                nodes.Add(new TopologyNode(fittingProxy.Position));
            }
            else if (proxy.Proxy is ISegmentProxy segmentProxy)
            {
                nodes.Add(ResolveSegmentNode(segmentProxy.Position, connectedProxies));
                nodes.Add(ResolveSegmentNode(segmentProxy.EndPosition, connectedProxies));
            }

            return nodes;
        }

        [Pure]
        private TopologyNode ResolveSegmentNode(Vector<double> segmentPoint, IEnumerable<IBoundaryProxy> connectedProxies)
        {
            foreach (IBoundaryProxy connectedProxy in connectedProxies)
            {
                bool isConnectedToPoint = connectedProxy.Boundary
                    .Any(bound => _comparer.Equals(bound, segmentPoint));
                if (!isConnectedToPoint)
                    continue;

                if (connectedProxy.Proxy is ISegmentProxy segmentProxy)
                {
                    if (_comparer.Equals(segmentProxy.Position, segmentPoint))
                        return new TopologyNode(segmentProxy.Position);
                    if (_comparer.Equals(segmentProxy.EndPosition, segmentPoint))
                        return new TopologyNode(segmentProxy.EndPosition);
                }

                if (connectedProxy.Proxy is IFittingProxy fittingProxy)
                    return new TopologyNode(fittingProxy.Position);
            }

            return new TopologyNode(segmentPoint);
        }
    }
}