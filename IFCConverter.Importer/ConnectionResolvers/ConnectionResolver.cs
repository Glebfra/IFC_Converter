using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.ConnectionResolvers
{
    internal sealed class ConnectionResolver
    {
        private static readonly Lazy<ConnectionResolver> _instance = new(() => new ConnectionResolver());

        public static ConnectionResolver GetInstance()
        {
            return _instance.Value;
        }

        [Pure]
        public IEnumerable<IBoundaryProxy> GetConnectedEntities(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> allProxies)
        {
            BoundPointConnectionResolver connectionResolver = new();
            return connectionResolver.GetConnectedEntities(proxy, allProxies);
        }

        [Pure]
        public IEnumerable<ITopologyEntity> GetConnectedEntities(ITopologyEntity entity, IEnumerable<ITopologyEntity> allTopologies)
        {
            TopologyConnectionResolver connectionResolver = new TopologyConnectionResolver();
            return connectionResolver.GetConnectedEntities(entity, allTopologies);
        }
    }
}