using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Start.Attributes;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.Extensions
{
    public static class AttributeExtensions
    {
        private static readonly Dictionary<Type, StartElementAttribute> StartElementCache = new Dictionary<Type, StartElementAttribute>();
        
        [Pure]
        public static StartElementAttribute GetStartElementAttribute(this IStartEntity entity)
        {
            Type type = entity.GetType();
            return StartElementCache.GetOrAdd(type, t => t.GetCustomAttribute<StartElementAttribute>());
        }
    }
}