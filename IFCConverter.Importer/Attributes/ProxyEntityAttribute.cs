using System;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    internal sealed class ProxyEntityAttribute : Attribute
    {
        private readonly Type? BoundaryResolverType;
        private readonly Type ConnectionResolverType;
        public readonly int ConnectionsCount;
        public readonly Type TopologyType;

        public ProxyEntityAttribute(int connectionsCount, Type topologyType, Type? boundaryResolverType = null, Type? connectionResolverType = null)
        {
            ConnectionsCount = connectionsCount;
            TopologyType = topologyType;
            BoundaryResolverType = boundaryResolverType;
            ConnectionResolverType = connectionResolverType ?? typeof(BoundPointConnectionResolver);
        }

        public IConnectionResolver GetConnectionResolver()
        {
            return ParameterlessConstructorRegistry<IConnectionResolver>.Create(ConnectionResolverType);
        }

        public IBoundaryResolver? GetBoundaryResolver()
        {
            if (BoundaryResolverType == null)
                return null;
            return ParameterlessConstructorRegistry<IBoundaryResolver>.Create(BoundaryResolverType);
        }
    }
}