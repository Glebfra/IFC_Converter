using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.FittingSegmentAugmenters
{
    internal sealed class ValveTopologySegmentAugmenter : ITopologySegmentAugmenter
    {
        private const double DoubleTolerance = 1e-3;
        private static readonly VectorComparer Comparer = new(DoubleTolerance);
        
        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity entity)
        {
            if (entity is not ValveTopologyEntity valveTopologyEntity)
                throw new InvalidOperationException($"{nameof(entity)}  must be of type {nameof(ValveTopologyEntity)}");
            
            return Augment(valveTopologyEntity);
        }

        public IEnumerable<ISegmentProxy> Augment(ValveTopologyEntity entity)
        {
            IReadOnlyCollection<SegmentTopologyEntity> connectedSegments = entity.Connected.OfType<SegmentTopologyEntity>().ToArray();
            AugmentConnectedSegments(connectedSegments, entity);
            
            return Enumerable.Empty<ISegmentProxy>();
        }

        private static void AugmentConnectedSegments(IEnumerable<SegmentTopologyEntity> connectedSegments, ValveTopologyEntity valveTopologyEntity)
        {
            ITopologyNodeEntity valveNode = valveTopologyEntity.Node;
            foreach (SegmentTopologyEntity connectedSegment in connectedSegments)
            {
                ITopologyNodeEntity startNode = connectedSegment.Nodes.ElementAt(0);
                ITopologyNodeEntity endNode = connectedSegment.Nodes.ElementAt(1);
                
                if (Comparer.NearerThan(startNode.Position, endNode.Position, valveNode.Position))
                {
                    startNode = valveNode;
                }
                else
                {
                    endNode = valveNode;
                }
                
                Vector<double> projection = endNode.Position - startNode.Position;
                connectedSegment.Augment(startNode, endNode, projection);
            }
        }
    }
}