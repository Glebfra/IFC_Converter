using System.Collections.Generic;

namespace IFCConverter.Interfaces
{
    internal interface ITopologyEntity
    {
        public IEntityProxy Proxy { get; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; }
        public IReadOnlyCollection<IEntityProxy> ConnectedProxies { get; }
    }
}