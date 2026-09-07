using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using IFCConverter.Utils.Collections;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Interfaces;

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