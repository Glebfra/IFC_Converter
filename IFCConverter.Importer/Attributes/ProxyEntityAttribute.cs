using System;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal sealed class ProxyEntityAttribute : Attribute
    {
        public readonly Type ConnectionResolverType;
        public readonly int ConnectionsCount;

        public ProxyEntityAttribute(Type connectionResolverType, int connectionsCount)
        {
            ConnectionResolverType = connectionResolverType;
            ConnectionsCount = connectionsCount;
        }
        
        public IEntityConnectionResolver GetConnectionResolver()
        {
            return (IEntityConnectionResolver)Activator.CreateInstance(ConnectionResolverType);
        }
    }
}