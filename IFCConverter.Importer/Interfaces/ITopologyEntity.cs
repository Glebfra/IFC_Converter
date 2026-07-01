using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Start.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologyEntity
    {
        public IBoundaryProxy Proxy { get; }
        public IReadOnlyCollection<ITopologyEntity> Connected { get; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; }

        public void Connect(ITopologyEntity topologyEntity);
        public void Connect(IEnumerable<ITopologyEntity> topologyEntities);

        [Pure]
        public IStartEntity ToStartEntity();
    }
}