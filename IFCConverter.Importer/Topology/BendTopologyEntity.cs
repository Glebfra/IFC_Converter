using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.FittingSegmentAugmenters;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Importer.Topology
{
    [TopologyEntity(typeof(BendTopologySegmentAugmenter))]
    internal sealed class BendTopologyEntity : TopologyEntity, IFittingTopologyEntity
    {
        private const double DoubleTolerance = 1e-3;
        private static readonly VectorComparer Comparer = new VectorComparer(DoubleTolerance);

        public BendTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes)
            : base(proxy, nodes)
        {
        }

        public ITopologyNodeEntity Node => Nodes.ElementAt(0);
    }
}