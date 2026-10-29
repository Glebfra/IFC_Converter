using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Collections;

namespace IFCConverter.Start.API
{
    public sealed class StartElementsRegistry
    {
        private readonly IDictionary<Type, StartElementTypeEnum> _elementTypesCache = new Dictionary<Type, StartElementTypeEnum>();
        private readonly IDictionary<Type, StartElementAttribute> _startElementCache = new Dictionary<Type, StartElementAttribute>();

        [Pure]
        public StartElementAttribute GetStartElementAttribute(IStartEntity entity)
        {
            Type type = entity.GetType();
            return _startElementCache.GetOrAdd(type, GetStartElementAttribute);
        }

        [Pure]
        public StartElementTypeEnum GetElementType(IStartEntity entity)
        {
            return _elementTypesCache.GetOrAdd(
                entity.GetType(),
                type => GetStartElementAttribute(type).Type
            );
        }

        [Pure]
        private static StartElementAttribute GetStartElementAttribute(Type type)
        {
            StartElementAttribute attribute = type.GetCustomAttribute<StartElementAttribute>();
            if (attribute == null)
                throw new ArgumentException($"Cannot find {nameof(StartElementAttribute)} for entity {type.Name}");

            return attribute;
        }
    }
}