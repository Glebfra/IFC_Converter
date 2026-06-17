using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.ConnectionResolvers
{
    internal sealed class ConnectionResolver
    {
        private static readonly Lazy<ConnectionResolver> _instance = new Lazy<ConnectionResolver>(() => new ConnectionResolver());

        public static ConnectionResolver GetInstance()
        {
            return _instance.Value;
        }
        
        [Pure]
        public IEnumerable<IBoundaryProxy> GetConnectedEntities(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> allProxies)
        {
            BoundPointConnectionResolver connectionResolver = new BoundPointConnectionResolver();
            return connectionResolver.GetConnectedEntities(proxy, allProxies);
        }
    }
}