using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.Entities.Topologies
{
    internal sealed class TopologyEntity : ITopologyEntity
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
        
    }
}