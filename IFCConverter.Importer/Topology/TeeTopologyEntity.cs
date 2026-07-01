using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.ConnectionAugmenters;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.Topology
{
    [TopologyEntity(typeof(TeeConnectionAugmenter))]
    internal sealed class TeeTopologyEntity : TopologyEntity, ISegmentAugmentableTopologyEntity, IFittingTopologyEntity
    {
        private const double DoubleTolerance = 1e-3;
        private static readonly VectorComparer Comparer = new VectorComparer(DoubleTolerance);
        
        public ITopologyNodeEntity Node => Nodes.ElementAt(0);
        
        public TeeTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes) : base(proxy, nodes)
        {
        }

        public void Augment()
        {
            ITopologyNodeEntity teeNode = Nodes.ElementAt(0);
            
            IEnumerable<SegmentTopologyEntity> segmentTopologyEntities = Connected.OfType<SegmentTopologyEntity>();
            foreach (SegmentTopologyEntity segmentTopologyEntity in segmentTopologyEntities)
            {
                ITopologyNodeEntity segmentStartNode = segmentTopologyEntity.Nodes.ElementAt(0);
                ITopologyNodeEntity segmentEndNode = segmentTopologyEntity.Nodes.ElementAt(1);

                if (Comparer.NearerThan(segmentStartNode.Position, segmentEndNode.Position, teeNode.Position))
                {
                    segmentStartNode = teeNode;
                }
                else
                {
                    segmentEndNode = teeNode;
                }

                Vector<double> projection = segmentEndNode.Position - segmentStartNode.Position;
                
                segmentTopologyEntity.Augment(segmentStartNode, segmentEndNode, projection);
            }
        }
    }
}