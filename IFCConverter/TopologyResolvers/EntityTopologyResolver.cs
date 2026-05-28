using System.Collections.Generic;
using System.Linq;
using IFCConverter.ConnectionResolvers;
using IFCConverter.Interfaces;
using IFCConverter.TopologyEntities;
using Utils;

namespace IFCConverter.TopologyResolvers
{
    internal sealed class EntityTopologyResolver : IEntityTopologyResolver
    {
        private readonly IEntityConnectionResolver _connectionResolver;
        private readonly INodeTopologyResolver _nodeResolver;

        public EntityTopologyResolver(VectorComparer comparer)
        {
            _connectionResolver = new BoundPointConnectionResolver(comparer);
            _nodeResolver = new NodeTopologyResolver(comparer);
        }

        public ITopologyEntity ResolveTopology(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies)
        {
            IReadOnlyCollection<IEntityProxy> connectedProxies = _connectionResolver
                .GetConnectedEntities(proxy, allProxies).ToArray();
            IReadOnlyCollection<ITopologyNodeEntity> nodes = _nodeResolver
                .ResolveTopology(proxy, connectedProxies);
            return new TopologyEntity(proxy, nodes, connectedProxies);
        }
    }
}