using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.Topology
{
    internal sealed class AttachmentTopologyEntity : TopologyEntity, IFittingTopologyEntity
    {
        public AttachmentTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes)
            : base(proxy, nodes)
        {
        }

        public ITopologyNodeEntity Node => Nodes.ElementAt(0);
    }
}