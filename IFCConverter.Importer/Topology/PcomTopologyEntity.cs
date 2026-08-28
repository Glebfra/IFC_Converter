using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Importer.Topology
{
    [TopologyEntity]
    internal sealed class PcomTopologyEntity : TopologyEntity, ISegmentTopologyEntity
    {
        public PcomTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes)
            : base(proxy, nodes)
        {
            Nodes = proxy.Boundary.Select(boundary => new TopologyNode(boundary)).ToArray();
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