using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Topology
{
    internal sealed class TopologyModelBuilder
    {
        private readonly BoundaryResolver _boundaryResolver = BoundaryResolver.GetInstance();
        private readonly ConnectionResolver _connectionResolver = ConnectionResolver.GetInstance();
        private readonly INodeTopologyResolver _nodeTopologyResolver;

        public TopologyModelBuilder(VectorComparer comparer)
        {
            _nodeTopologyResolver = new NodeTopologyResolver(comparer);
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

            return new TopologyModel(result, resultNodes);
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