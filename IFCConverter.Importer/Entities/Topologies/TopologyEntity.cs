using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.Entities.Topologies
{
    internal sealed class TopologyEntity : ITopologyEntity
    {
        public TopologyEntity(
            IEntityProxy proxy,
            IReadOnlyCollection<ITopologyNodeEntity> nodes,
            IReadOnlyCollection<IEntityProxy> connectedProxies)
        {
            Proxy = proxy;
            Nodes = nodes;
            ConnectedProxies = connectedProxies;
        }

        public IEntityProxy Proxy { get; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; }
        public IReadOnlyCollection<IEntityProxy> ConnectedProxies { get; }
    }
}