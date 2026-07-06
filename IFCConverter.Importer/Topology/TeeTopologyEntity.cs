using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.FittingSegmentAugmenters;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Topology
{
    [TopologyEntity(typeof(TeeTopologySegmentAugmenter))]
    internal sealed class TeeTopologyEntity : TopologyEntity, IFittingTopologyEntity
    {
        private const double DoubleTolerance = 1e-3;
        private static readonly VectorComparer Comparer = new(DoubleTolerance);

        public TeeTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes) : base(proxy, nodes)
        {
        }

        public ITopologyNodeEntity Node => Nodes.ElementAt(0);
    }
}