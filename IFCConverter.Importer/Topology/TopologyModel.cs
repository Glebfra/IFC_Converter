using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.Topology
{
    /// <summary>
    ///     Represents the topology model containing all topology entities and their
    ///     connectivity information. The model is responsible for creating topology
    ///     entities, resolving boundaries, calculating connections, and augmenting
    ///     segment-based entities.
    /// </summary>
    /// <remarks>
    ///     Connections between entities are automatically recalculated whenever
    ///     entities are added to the model.
    /// </remarks>
    internal sealed class TopologyModel : ITopologyModel
    {
        private const double DefaultDoubleTolerance = 1e-3;
        private static readonly VectorComparer DefaultComparer = new(DefaultDoubleTolerance);

        private static readonly BoundaryResolver BoundaryResolver = BoundaryResolver.GetInstance();
        private static readonly ConnectionResolver ConnectionResolver = ConnectionResolver.GetInstance();
        private readonly List<ITopologyEntity> _entities;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TopologyModel" /> class
        ///     using an existing collection of topology entities.
        /// </summary>
        /// <param name="entities">
        ///     The topology entities that will compose the model.
        /// </param>
        public TopologyModel(IEnumerable<ITopologyEntity> entities)
        {
            _entities = entities.ToList();
            RecalculateConnections();
            Augment();
        }

        /// <summary>
        ///     Gets all topology entities contained in the model.
        /// </summary>
        public IEnumerable<ITopologyEntity> Entities => _entities;

        /// <summary>
        ///     Adds a new entity proxy to the topology model.
        /// </summary>
        /// <param name="proxy">
        ///     The entity proxy to add.
        /// </param>
        /// <remarks>
        ///     The boundary is automatically resolved before the corresponding
        ///     topology entity is created and added to the model.
        ///     Connections are recalculated after the entity is added.
        /// </remarks>
        public void AddEntity(IEntityProxy proxy)
        {
            IReadOnlyCollection<IEntityProxy> allProxies = _entities.Select(entity => entity.Proxy.Proxy).ToArray();
            IReadOnlyCollection<Vector<double>> boundary = BoundaryResolver.ResolveBoundary(proxy, allProxies);

            IBoundaryProxy boundaryProxy = new BoundaryProxy(proxy, boundary);
            AddEntity(boundaryProxy);
        }

        /// <summary>
        ///     Adds multiple entity proxies to the topology model.
        /// </summary>
        /// <param name="proxies">
        ///     The entity proxies to add.
        /// </param>
        /// <remarks>
        ///     Boundaries are resolved using both the existing entities and the
        ///     provided proxies to ensure consistent connectivity.
        ///     Connections are recalculated after all entities have been added.
        /// </remarks>
        public void AddEntities(IEnumerable<IEntityProxy> proxies)
        {
            IReadOnlyCollection<IEntityProxy> proxiesArray = proxies as IReadOnlyCollection<IEntityProxy> ?? proxies.ToArray();

            List<IEntityProxy> allProxies = _entities.Select(entity => entity.Proxy.Proxy).ToList();
            allProxies.AddRange(proxiesArray);

            List<IBoundaryProxy> boundaryProxies = new();
            foreach (IEntityProxy proxy in proxiesArray)
            {
                IReadOnlyCollection<Vector<double>> boundary = BoundaryResolver.ResolveBoundary(proxy, allProxies);
                IBoundaryProxy boundaryProxy = new BoundaryProxy(proxy, boundary);
                boundaryProxies.Add(boundaryProxy);
            }

            AddEntities(boundaryProxies);
        }

        /// <summary>
        ///     Adds a boundary proxy to the topology model.
        /// </summary>
        /// <param name="boundaryProxy">
        ///     The boundary proxy to add.
        /// </param>
        /// <remarks>
        ///     The boundary proxy is converted into its corresponding topology entity
        ///     before being added to the model.
        /// </remarks>
        public void AddEntity(IBoundaryProxy boundaryProxy)
        {
            ITopologyEntity topologyEntity = CreateTopologyEntity(boundaryProxy);
            AddEntity(topologyEntity);
        }

        /// <summary>
        ///     Adds multiple boundary proxies to the topology model.
        /// </summary>
        /// <param name="boundaryProxies">
        ///     The boundary proxies to add.
        /// </param>
        public void AddEntities(IEnumerable<IBoundaryProxy> boundaryProxies)
        {
            IEnumerable<ITopologyEntity> topologyEntities = boundaryProxies.Select(boundaryProxy => CreateTopologyEntity(boundaryProxy));
            AddEntities(topologyEntities);
        }

        /// <summary>
        ///     Adds an existing topology entity to the model.
        /// </summary>
        /// <param name="entity">
        ///     The topology entity to add.
        /// </param>
        /// <remarks>
        ///     Connectivity is recalculated after the entity has been added.
        /// </remarks>
        public void AddEntity(ITopologyEntity entity)
        {
            _entities.Add(entity);
            RecalculateConnections();
        }

        /// <summary>
        ///     Adds multiple topology entities to the model.
        /// </summary>
        /// <param name="entities">
        ///     The topology entities to add.
        /// </param>
        /// <remarks>
        ///     Connectivity is recalculated after all entities have been added.
        /// </remarks>
        public void AddEntities(IEnumerable<ITopologyEntity> entities)
        {
            _entities.AddRange(entities);
            RecalculateConnections();
        }

        /// <summary>
        ///     Creates a topology model from a collection of boundary proxies.
        /// </summary>
        /// <param name="boundaryProxies">
        ///     The boundary proxies used to create topology entities.
        /// </param>
        /// <param name="comparer">
        ///     The vector comparer used for geometric comparisons.
        ///     If <see langword="null" />, the default comparer is used.
        /// </param>
        /// <returns>
        ///     A fully initialized <see cref="TopologyModel" />.
        /// </returns>
        public static TopologyModel Create(IEnumerable<IBoundaryProxy> boundaryProxies, VectorComparer? comparer = null)
        {
            IReadOnlyCollection<IBoundaryProxy> boundaryProxiesArray = boundaryProxies as IReadOnlyCollection<IBoundaryProxy> ?? boundaryProxies.ToArray();

            comparer ??= DefaultComparer;
            IEnumerable<ITopologyEntity> result = CreateTopologyEntities(boundaryProxiesArray);

            return new TopologyModel(result);
        }

        /// <summary>
        ///     Creates a topology model from a collection of entity proxies.
        /// </summary>
        /// <param name="proxies">
        ///     The entity proxies from which boundaries and topology entities are created.
        /// </param>
        /// <param name="comparer">
        ///     The vector comparer used for geometric comparisons.
        ///     If <see langword="null" />, the default comparer is used.
        /// </param>
        /// <returns>
        ///     A fully initialized <see cref="TopologyModel" />.
        /// </returns>
        public static TopologyModel Create(IEnumerable<IEntityProxy> proxies, VectorComparer? comparer = null)
        {
            IReadOnlyCollection<IEntityProxy> proxiesArray = proxies as IReadOnlyCollection<IEntityProxy> ?? proxies.ToArray();

            IReadOnlyCollection<IBoundaryProxy> boundaryProxies = proxiesArray
                .Select(proxy => new BoundaryProxy(proxy, BoundaryResolver.ResolveBoundary(proxy, proxiesArray)))
                .ToArray();

            return Create(boundaryProxies, comparer);
        }

        private static IEnumerable<ITopologyEntity> CreateTopologyEntities(IReadOnlyCollection<IBoundaryProxy> boundaryProxies)
        {
            Dictionary<IBoundaryProxy, IEnumerable<IBoundaryProxy>> connections = new();
            Dictionary<IBoundaryProxy, ITopologyEntity> proxyToTopologyMap = new();

            foreach (IBoundaryProxy boundaryProxy in boundaryProxies)
            {
                IReadOnlyCollection<IBoundaryProxy> connected = ConnectionResolver.GetConnectedEntities(boundaryProxy, boundaryProxies).ToArray();
                connections.Add(boundaryProxy, connected);

                ITopologyEntity topologyEntity = CreateTopologyEntity(boundaryProxy);
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

        private static ITopologyEntity CreateTopologyEntity(IBoundaryProxy boundaryProxy)
        {
            IReadOnlyCollection<ITopologyNodeEntity> nodes;
            if (boundaryProxy.Proxy is ISegmentProxy segmentProxy)
                nodes = new ITopologyNodeEntity[]
                {
                    new TopologyNode(segmentProxy.Position), new TopologyNode(segmentProxy.EndPosition)
                };
            else
            {
                nodes = new ITopologyNodeEntity[]
                {
                    new TopologyNode(boundaryProxy.Proxy.Position)
                };
            }

            ProxyEntityAttribute attribute = boundaryProxy.Proxy.GetProxyEntityAttribute();
            Type topologyType = attribute.TopologyType;
            return ConstructorRegistry.CreateTopologyEntity(topologyType, boundaryProxy, nodes);
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