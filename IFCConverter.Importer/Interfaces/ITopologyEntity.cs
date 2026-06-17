using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologyEntity
    {
        public IBoundaryProxy Proxy { get; }
        public IReadOnlyCollection<IBoundaryProxy> ConnectedProxies { get; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; }
    }
}