using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Entities.Topologies
{
    internal sealed class EntityTopologyResolver : IEntityTopologyResolver
    {
        private readonly ConnectionResolver _connectionResolver;
        private readonly INodeTopologyResolver _nodeResolver;

        public EntityTopologyResolver(VectorComparer comparer)
        {
            _connectionResolver = ConnectionResolver.GetInstance();
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