using System;
using System.Collections.Generic;
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

        public IEnumerable<IEntityProxy> GetConnectedEntities(
            IEntityProxy proxy,
            IReadOnlyCollection<IEntityProxy> allProxies)
        {
            ProxyEntityAttribute attribute = proxy.GetType().GetCustomAttribute<ProxyEntityAttribute>();
            IEntityConnectionResolver connectionResolver = attribute.GetConnectionResolver();
            int connectionResolverCount = allProxies.Count;
            
            return connectionResolver.GetConnectedEntities(proxy, allProxies, connectionResolverCount);
        }

        public bool IsConnected(IEntityProxy first, IEntityProxy second)
        {
            Type firstType = first.GetType();
            Type secondType = second.GetType();

            ProxyEntityAttribute firstProxyAttribute = firstType.GetCustomAttribute<ProxyEntityAttribute>();
            ProxyEntityAttribute secondProxyAttribute = secondType.GetCustomAttribute<ProxyEntityAttribute>();

            throw new NotImplementedException();
        }
    }
}