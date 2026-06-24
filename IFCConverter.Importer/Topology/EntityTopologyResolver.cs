using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Topology
{
    internal sealed class EntityTopologyResolver : IEntityTopologyResolver
    {
        private readonly BoundaryResolver _boundaryResolver = BoundaryResolver.GetInstance();
        private readonly ConnectionResolver _connectionResolver = ConnectionResolver.GetInstance();
        private readonly NodeTopologyResolver _nodeTopologyResolver;

        public EntityTopologyResolver(VectorComparer comparer)
        {
            _nodeTopologyResolver = new NodeTopologyResolver(comparer);
        }

        public IReadOnlyCollection<ITopologyEntity> ResolveTopologies(IReadOnlyCollection<IEntityProxy> allProxies)
        {
            IReadOnlyCollection<IBoundaryProxy> boundaryProxies = allProxies
                .Select(proxy => new BoundaryProxy(proxy, _boundaryResolver.ResolveBoundary(proxy, allProxies)))
                .ToArray();

            List<ITopologyEntity> result = new();
            foreach (IBoundaryProxy boundaryProxy in boundaryProxies)
            {
                IReadOnlyCollection<IBoundaryProxy> connected = _connectionResolver.GetConnectedEntities(boundaryProxy, boundaryProxies).ToArray();
                IReadOnlyCollection<ITopologyNodeEntity> nodes = _nodeTopologyResolver.ResolveTopology(boundaryProxy, connected).ToArray();
                result.Add(CreateTopologyEntity(boundaryProxy, nodes, connected));
            }

            return result;
        }

        private ITopologyEntity CreateTopologyEntity(
            IBoundaryProxy proxy,
            IReadOnlyCollection<ITopologyNodeEntity> nodes,
            IReadOnlyCollection<IBoundaryProxy> connected)
        {
            if (proxy.Proxy is ValveProxy valveProxy)
                return new ValveTopologyEntity(proxy, nodes, connected);
            
            return new TopologyEntity(proxy, nodes, connected);
        }
    }
}