using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Start.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologyEntity
    {
        public IBoundaryProxy Proxy { get; }
        public IReadOnlyCollection<IBoundaryProxy> ConnectedProxies { get; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; }

        [Pure]
        public IStartEntity ToStartEntity();
    }
}