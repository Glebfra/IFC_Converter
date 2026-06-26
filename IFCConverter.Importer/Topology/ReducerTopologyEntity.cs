using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.ConnectionAugmenters;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.Topology
{
    [TopologyEntity(typeof(ReducerConnectionAugmenter))]
    internal sealed class ReducerTopologyEntity : TopologyEntity, ISegmentAugmentableTopologyEntity
    {
        private const double DoubleTolerance = 1e-3;
        private static readonly VectorComparer Comparer = new VectorComparer(DoubleTolerance);
        
        public ReducerTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes) : base(proxy, nodes)
        {
        }

        public ReducerTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes, IReadOnlyCollection<IBoundaryProxy> connectedProxies) : base(proxy, nodes, connectedProxies)
        {
        }

        public void Augment()
        {
            ITopologyNodeEntity reducerNode = Nodes.ElementAt(0);

            IEnumerable<SegmentTopologyEntity> segmentTopologyEntities = Connected.OfType<SegmentTopologyEntity>();
            foreach (SegmentTopologyEntity segmentTopologyEntity in segmentTopologyEntities)
            {
                ITopologyNodeEntity segmentStartNode = segmentTopologyEntity.Nodes.ElementAt(0);
                ITopologyNodeEntity segmentEndNode = segmentTopologyEntity.Nodes.ElementAt(1);

                Vector<double> previousProjection = segmentEndNode.Position - segmentStartNode.Position;

                if (Comparer.NearerThan(segmentStartNode.Position, segmentEndNode.Position, reducerNode.Position))
                {
                    segmentStartNode = reducerNode;
                }
                else
                {
                    segmentEndNode = reducerNode;
                }

                Vector<double> newProjection = segmentEndNode.Position - segmentStartNode.Position;

                Vector<double> projection = newProjection.DotProduct(previousProjection.Normalize(2)) * previousProjection.Normalize(2);
                
                segmentTopologyEntity.Augment(segmentStartNode, segmentEndNode, projection);
            }
        }
    }
}