using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Importer.Interfaces;
using Start.Interfaces;

namespace IFCConverter.Importer.Topology
{
    internal class TopologyEntity : ITopologyEntity
    {
        public TopologyEntity(
            IBoundaryProxy proxy,
            IReadOnlyCollection<ITopologyNodeEntity> nodes,
            IReadOnlyCollection<IBoundaryProxy> connectedProxies
        )
        {
            Proxy = proxy;
            Nodes = nodes;
            ConnectedProxies = connectedProxies;
        }

        public IBoundaryProxy Proxy { get; }
        public IReadOnlyCollection<IBoundaryProxy> ConnectedProxies { get; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; }

        [Pure]
        public virtual IStartEntity ToStartEntity()
        {
            return Proxy.ToStartEntity();
        }
    }
}