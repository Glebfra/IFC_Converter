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
            IReadOnlyCollection<ITopologyNodeEntity> nodes)
        {
            Proxy = proxy;
            Nodes = nodes;
        }
        
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

        public IBoundaryProxy Proxy { get; protected set; }
        public IReadOnlyCollection<IBoundaryProxy> ConnectedProxies { get; protected set; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; protected set; }

        public IReadOnlyCollection<ITopologyEntity> Connected => _connected;
        private readonly List<ITopologyEntity> _connected = new List<ITopologyEntity>();

        public void Connect(ITopologyEntity topologyEntity)
        {
            _connected.Add(topologyEntity);
        }

        public void Connect(IEnumerable<ITopologyEntity> topologyEntities)
        {
            _connected.AddRange(topologyEntities);
        }

        [Pure]
        public virtual IStartEntity ToStartEntity()
        {
            return Proxy.ToStartEntity();
        }
    }
}