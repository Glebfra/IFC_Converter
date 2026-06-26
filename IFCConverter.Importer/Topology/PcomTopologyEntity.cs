using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using Start.Entities.Segments;
using Start.Interfaces;

namespace IFCConverter.Importer.Topology
{
    internal sealed class PcomTopologyEntity : TopologyEntity
    {
        public PcomTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes) 
            : base(proxy, nodes)
        {
            Nodes = proxy.Boundary.Select(boundary => new TopologyNode(boundary)).ToArray();
        }

        public PcomTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes, IReadOnlyCollection<IBoundaryProxy> connectedProxies) 
            : base(proxy, nodes, connectedProxies)
        {
        }

        public override IStartEntity ToStartEntity()
        {
            StartRigidElementEntity startRigidElementEntity = (StartRigidElementEntity)base.ToStartEntity();
            startRigidElementEntity.Projection = Nodes.ElementAt(1).Position - Nodes.ElementAt(0).Position;
            startRigidElementEntity.StartPosition = Nodes.ElementAt(0).Position;
            return startRigidElementEntity;
        }
    }
}