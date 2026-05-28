using System.Linq;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Entities.Proxies
{
    internal sealed class SegmentResolver
    {
        private readonly VectorComparer _comparer;

        public SegmentResolver(VectorComparer comparer)
        {
            _comparer = comparer;
        }

        public IResolvedSegmentProxy Resolve(ITopologyEntity topologyEntity)
        {
            ISegmentProxy segmentProxy = (ISegmentProxy)topologyEntity.Proxy;
            ITopologyNodeEntity[] nodes = topologyEntity.Nodes.ToArray();

            return ResolvedSegmentProxy.CreateFromSegmentProxy(segmentProxy, nodes[0].Position, nodes[1].Position);
        }
    }
}