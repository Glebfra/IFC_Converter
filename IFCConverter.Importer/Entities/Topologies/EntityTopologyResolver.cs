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
        private readonly IEntityConnectionResolver _connectionResolver;
        private readonly INodeTopologyResolver _nodeResolver;

        public EntityTopologyResolver(VectorComparer comparer)
        {
            _connectionResolver = new BoundPointConnectionResolver();
            _nodeResolver = new NodeTopologyResolver(comparer);
        }

        public ITopologyEntity ResolveTopology(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies)
        {
            ProxyEntityAttribute? proxyEntityAttribute = proxy.GetType().GetCustomAttribute<ProxyEntityAttribute>();
            if (proxyEntityAttribute == null)
                throw new Exception(
                    $"Proxy entity attribute is missing for proxy of type {proxy.GetType().Name}"
                );
            Type connectionResolverType = proxyEntityAttribute.ConnectionResolverType;

            IReadOnlyCollection<IEntityProxy> connectedProxies = _connectionResolver
                .GetConnectedEntities(proxy, allProxies).ToArray();
            IReadOnlyCollection<ITopologyNodeEntity> nodes = _nodeResolver
                .ResolveTopology(proxy, connectedProxies);
            return new TopologyEntity(proxy, nodes, connectedProxies);
        }
    }
}