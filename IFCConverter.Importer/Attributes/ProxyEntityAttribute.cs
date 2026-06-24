using System;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    internal sealed class ProxyEntityAttribute : Attribute
    {
        public readonly Type? BoundaryResolverType;
        public readonly Type? ConnectionAugmenterType;
        public readonly int ConnectionsCount;

        public ProxyEntityAttribute(int connectionsCount, Type? connectionAugmenterType = null,
            Type? boundaryResolverType = null)
        {
            ConnectionsCount = connectionsCount;
            BoundaryResolverType = boundaryResolverType;
            ConnectionAugmenterType = connectionAugmenterType;
        }

        public IEntityConnectionAugmenter? GetConnectionAugmenter()
        {
            if (ConnectionAugmenterType == null)
                return null;
            return (IEntityConnectionAugmenter)Activator.CreateInstance(ConnectionAugmenterType);
        }

        public IBoundaryResolver? GetBoundaryResolver()
        {
            if (BoundaryResolverType == null)
                return null;
            return (IBoundaryResolver)Activator.CreateInstance(BoundaryResolverType);
        }
    }
}