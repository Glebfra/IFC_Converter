using System;
using System.Collections.Generic;
using System.Reflection;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Utils.Collections;

namespace IFCConverter.Importer.Extensions
{
    internal static class AttributeExtensions
    {
        private static readonly Dictionary<Type, ProxyEntityAttribute> ProxyEntityCache = new Dictionary<Type, ProxyEntityAttribute>();
        private static readonly Dictionary<Type, TopologyEntityAttribute> TopologyEntityCache = new Dictionary<Type, TopologyEntityAttribute>();

        public static ProxyEntityAttribute GetProxyEntityAttribute(this IEntityProxy entityProxy)
        {
            Type type = entityProxy.GetType();
            return ProxyEntityCache.GetOrAdd(type, t => t.GetCustomAttribute<ProxyEntityAttribute>());
        }

        public static TopologyEntityAttribute GetTopologyEntityAttribute(this ITopologyEntity topologyEntity)
        {
            Type type = topologyEntity.GetType();
            return TopologyEntityCache.GetOrAdd(type, t => t.GetCustomAttribute<TopologyEntityAttribute>());
        }
    }
}