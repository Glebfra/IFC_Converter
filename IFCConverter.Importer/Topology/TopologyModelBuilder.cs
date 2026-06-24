using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using Utils;

namespace IFCConverter.Importer.Topology
{
    internal sealed class TopologyModelBuilder
    {
        private readonly BoundaryResolver _boundaryResolver = BoundaryResolver.GetInstance();
        private readonly ConnectionResolver _connectionResolver = ConnectionResolver.GetInstance();
        private readonly SegmentResolver _segmentResolver;
        private readonly INodeTopologyResolver _nodeTopologyResolver;

        public TopologyModelBuilder(VectorComparer comparer)
        {
            _nodeTopologyResolver = new NodeTopologyResolver(comparer);
            _segmentResolver = new SegmentResolver(comparer);
        }

        public ITopologyModel Build(IReadOnlyCollection<IEntityProxy> proxies)
        {
            IReadOnlyCollection<IBoundaryProxy> boundaryProxies = proxies
                .Select(proxy => new BoundaryProxy(proxy, _boundaryResolver.ResolveBoundary(proxy, proxies)))
                .ToArray();
            
            List<ITopologyEntity> result = new();
            List<ITopologyNodeEntity> resultNodes = new List<ITopologyNodeEntity>();
            foreach (IBoundaryProxy boundaryProxy in boundaryProxies)
            {
                IReadOnlyCollection<IBoundaryProxy> connected = _connectionResolver.GetConnectedEntities(boundaryProxy, boundaryProxies).ToArray();
                IReadOnlyCollection<ITopologyNodeEntity> nodes = _nodeTopologyResolver.ResolveTopology(boundaryProxy, connected).ToArray();
                
                result.Add(CreateTopologyEntity(boundaryProxy, nodes, connected));
                resultNodes.AddRange(nodes);
            }
            ResolveSegments(ref result);

            return new TopologyModel(result, resultNodes);
        }

        private void ResolveSegments(ref List<ITopologyEntity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                ITopologyEntity topologyEntity = entities[i];
                if (topologyEntity.Proxy.Proxy is not ISegmentProxy)
                    continue;

                entities[i] = _segmentResolver.Resolve(topologyEntity);
            }
        }
        
        private static ITopologyEntity CreateTopologyEntity(
            IBoundaryProxy proxy, 
            IReadOnlyCollection<ITopologyNodeEntity> nodes,
            IReadOnlyCollection<IBoundaryProxy> connectedProxies)
        {
            return proxy.Proxy switch
            {
                ValveProxy => new ValveTopologyEntity(proxy, nodes, connectedProxies),
                _ => new TopologyEntity(proxy, nodes, connectedProxies)
            };
        }
    }
}