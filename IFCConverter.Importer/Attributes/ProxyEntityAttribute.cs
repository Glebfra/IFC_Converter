using System;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal sealed class ProxyEntityAttribute : Attribute
    {
        public readonly Type ConnectionResolverType;
        public readonly Type? ConnectionAugmenterType;
        public readonly Type? BoundaryResolverType;
        public readonly int ConnectionsCount;

        public ProxyEntityAttribute(Type connectionResolverType, int connectionsCount, Type? connectionAugmenterType = null, Type? boundaryResolverType = null)
        {
            ConnectionResolverType = connectionResolverType;
            ConnectionsCount = connectionsCount;
            BoundaryResolverType = boundaryResolverType;
            ConnectionAugmenterType = connectionAugmenterType;
        }
        
        public IEntityConnectionResolver GetConnectionResolver()
        {
            return (IEntityConnectionResolver)Activator.CreateInstance(ConnectionResolverType);
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