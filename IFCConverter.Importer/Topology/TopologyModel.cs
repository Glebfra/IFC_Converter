using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using Utils;

namespace IFCConverter.Importer.Topology
{
    internal sealed class TopologyModel : ITopologyModel
    {
        private const double DefaultDoubleTolerance = 1e-3;
        private static readonly VectorComparer DefaultComparer = new(DefaultDoubleTolerance);

        private static readonly BoundaryResolver BoundaryResolver = BoundaryResolver.GetInstance();
        private static readonly ConnectionResolver ConnectionResolver = ConnectionResolver.GetInstance();
        private readonly List<ITopologyEntity> _entities;

        public TopologyModel(IEnumerable<ITopologyEntity> entities)
        {
            _entities = entities.ToList();
            RecalculateConnections();
            Augment();
        }

        public IEnumerable<ITopologyEntity> Entities => _entities;

        public void AddEntity(ITopologyEntity entity)
        {
            _entities.Add(entity);
            RecalculateConnections();
        }

        public void AddEntities(IEnumerable<ITopologyEntity> entities)
        {
            _entities.AddRange(entities);
            RecalculateConnections();
        }

        public static TopologyModel Create(IEnumerable<IBoundaryProxy> boundaryProxies, VectorComparer? comparer = null)
        {
            IReadOnlyCollection<IBoundaryProxy> boundaryProxiesArray = boundaryProxies as IReadOnlyCollection<IBoundaryProxy> ?? boundaryProxies.ToArray();

            comparer ??= DefaultComparer;
            INodeTopologyResolver nodeTopologyResolver = new NodeTopologyResolver(comparer);
            IEnumerable<ITopologyEntity> result = CreateTopologyEntities(boundaryProxiesArray, nodeTopologyResolver);

            return new TopologyModel(result);
        }

        public static TopologyModel Create(IEnumerable<IEntityProxy> proxies, VectorComparer? comparer = null)
        {
            IReadOnlyCollection<IEntityProxy> proxiesArray = proxies as IReadOnlyCollection<IEntityProxy> ?? proxies.ToArray();

            IReadOnlyCollection<IBoundaryProxy> boundaryProxies = proxiesArray
                .Select(proxy => new BoundaryProxy(proxy, BoundaryResolver.ResolveBoundary(proxy, proxiesArray)))
                .ToArray();

            return Create(boundaryProxies, comparer);
        }

        private static IEnumerable<ITopologyEntity> CreateTopologyEntities(
            IReadOnlyCollection<IBoundaryProxy> boundaryProxies,
            INodeTopologyResolver nodeTopologyResolver)
        {
            Dictionary<IBoundaryProxy, IEnumerable<IBoundaryProxy>> connections = new();
            Dictionary<IBoundaryProxy, ITopologyEntity> proxyToTopologyMap = new();

            foreach (IBoundaryProxy boundaryProxy in boundaryProxies)
            {
                IReadOnlyCollection<IBoundaryProxy> connected = ConnectionResolver.GetConnectedEntities(boundaryProxy, boundaryProxies).ToArray();
                connections.Add(boundaryProxy, connected);

                IReadOnlyCollection<ITopologyNodeEntity> nodes = nodeTopologyResolver.ResolveTopologyRaw(boundaryProxy, connected).ToArray();

                ProxyEntityAttribute attribute = boundaryProxy.Proxy.GetType().GetCustomAttribute<ProxyEntityAttribute>();
                Type topologyType = attribute.TopologyType;
                ITopologyEntity topologyEntity = (ITopologyEntity)Activator.CreateInstance(topologyType, boundaryProxy, nodes);

                proxyToTopologyMap.Add(boundaryProxy, topologyEntity);
            }

            IReadOnlyCollection<ITopologyEntity> topologyEntities = proxyToTopologyMap.Values;
            foreach (ITopologyEntity topologyEntity in topologyEntities)
            {
                IEnumerable<ITopologyEntity> connected = connections[topologyEntity.Proxy].Select(proxy => proxyToTopologyMap[proxy]);
                topologyEntity.Connect(connected);
            }

            return topologyEntities;
        }

        private void RecalculateConnections()
        {
            foreach (ITopologyEntity topologyEntity in _entities)
            {
                IEnumerable<ITopologyEntity> connectedEntities = ConnectionResolver.GetConnectedEntities(topologyEntity, _entities);
                IEnumerable<ITopologyEntity> notConnectedEntities = connectedEntities.Where(connected => !topologyEntity.Connected.Contains(connected));
                topologyEntity.Connect(notConnectedEntities);
            }
        }

        private void Augment()
        {
            foreach (ISegmentAugmentableTopologyEntity segmentAugmentableTopologyEntity in _entities.OfType<ISegmentAugmentableTopologyEntity>())
            {
                segmentAugmentableTopologyEntity.Augment();
            }
        }
    }
}