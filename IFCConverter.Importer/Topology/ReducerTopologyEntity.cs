using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.FittingSegmentAugmenters;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Importer.Topology
{
    [TopologyEntity(typeof(ReducerTopologySegmentAugmenter))]
    internal sealed class ReducerTopologyEntity : TopologyEntity, IFittingTopologyEntity
    {
        private const double DoubleTolerance = 1e-3;
        private static readonly VectorComparer Comparer = new VectorComparer(DoubleTolerance);

        public ReducerTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes) : base(proxy, nodes)
        {
        }

        public ITopologyNodeEntity Node => Nodes.ElementAt(0);
    }
}