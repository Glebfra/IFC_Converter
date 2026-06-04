using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    public interface ITopologyEntity
    {
        public IEntityProxy Proxy { get; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; }
        public IReadOnlyCollection<IEntityProxy> ConnectedProxies { get; }
    }
}