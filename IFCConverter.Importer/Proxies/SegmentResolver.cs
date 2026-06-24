using System.Linq;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using Utils;

namespace IFCConverter.Importer.Proxies
{
    internal sealed class SegmentResolver
    {
        private readonly VectorComparer _comparer;

        public SegmentResolver(VectorComparer comparer)
        {
            _comparer = comparer;
        }

        public SegmentTopologyEntity Resolve(ITopologyEntity topologyEntity)
        {
            ISegmentProxy segmentProxy = (ISegmentProxy)topologyEntity.Proxy.Proxy;
            ITopologyNodeEntity[] nodes = topologyEntity.Nodes.ToArray();

            return new SegmentTopologyEntity(topologyEntity.Proxy, nodes, topologyEntity.ConnectedProxies);
        }
    }
}