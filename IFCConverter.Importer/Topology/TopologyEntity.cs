using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Importer.Interfaces;
using Start.Interfaces;

namespace IFCConverter.Importer.Topology
{
    internal class TopologyEntity : ITopologyEntity
    {
        private readonly List<ITopologyEntity> _connected = new();

        public TopologyEntity(
            IBoundaryProxy proxy,
            IReadOnlyCollection<ITopologyNodeEntity> nodes)
        {
            Proxy = proxy;
            Nodes = nodes;
        }

        public IBoundaryProxy Proxy { get; protected set; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; protected set; }

        public IReadOnlyCollection<ITopologyEntity> Connected => _connected;

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