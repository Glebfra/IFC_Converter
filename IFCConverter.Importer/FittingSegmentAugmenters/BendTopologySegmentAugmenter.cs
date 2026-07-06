using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.FittingSegmentAugmenters
{
    internal sealed class BendTopologySegmentAugmenter : ITopologySegmentAugmenter
    {
        private const double DoubleTolerance = 1e-3;
        private static readonly VectorComparer Comparer = new(DoubleTolerance);
        
        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity entity)
        {
            if (entity is not BendTopologyEntity bendTopologyEntity)
                throw new InvalidOperationException($"{nameof(entity)}  must be of type {nameof(BendTopologyEntity)}");
            
            return Augment(bendTopologyEntity);
        }

        public IEnumerable<ISegmentProxy> Augment(BendTopologyEntity entity)
        {
            IReadOnlyCollection<SegmentTopologyEntity> connectedSegments = entity.Connected.OfType<SegmentTopologyEntity>().ToArray();
            AugmentConnectedSegments(connectedSegments, entity);
            
            IEnumerable<ISegmentProxy> result = GenerateConnectedSegments(connectedSegments, entity);
            return result;
        }

        private static void AugmentConnectedSegments(IEnumerable<SegmentTopologyEntity> connectedSegments, BendTopologyEntity bendTopologyEntity)
        {
            ITopologyNodeEntity bendNode = bendTopologyEntity.Node;
            foreach (SegmentTopologyEntity connectedSegment in connectedSegments)
            {
                ITopologyNodeEntity startNode = connectedSegment.Nodes.ElementAt(0);
                ITopologyNodeEntity endNode = connectedSegment.Nodes.ElementAt(1);
                
                if (Comparer.NearerThan(startNode.Position, endNode.Position, bendNode.Position))
                {
                    startNode = bendNode;
                }
                else
                {
                    endNode = bendNode;
                }
                
                Vector<double> projection = endNode.Position - startNode.Position;
                connectedSegment.Augment(startNode, endNode, projection);
            }
        }

        private static IEnumerable<ISegmentProxy> GenerateConnectedSegments(
            IEnumerable<SegmentTopologyEntity> existingSegments,
            BendTopologyEntity bendTopologyEntity)
        {
            BendProxy bendProxy = (BendProxy)bendTopologyEntity.Proxy.Proxy;
            List<ISegmentProxy> result = new List<ISegmentProxy>();
            
            ITopologyNodeEntity bendNode = bendTopologyEntity.Node;
            IReadOnlyCollection<Vector<double>> segmentDirections = existingSegments.Select(segment => segment.Projection.Normalize(2)).ToArray();
            IReadOnlyCollection<Vector<double>> boundaries = bendTopologyEntity.Proxy.Boundary;
            foreach (Vector<double> boundary in boundaries)
            {
                Vector<double> projectionToBoundary = boundary - bendNode.Position;
                double projectionLength = projectionToBoundary.L2Norm();
                Vector<double> directionToBoundary = projectionToBoundary / projectionLength;
                bool isAlreadyConnected = segmentDirections.Any(direction => direction.IsParallel(directionToBoundary, DoubleTolerance));
                if (isAlreadyConnected)
                    continue;

                ISegmentProxy newSegmentProxy = new PipeSegmentProxy(
                    bendProxy.Diameter,
                    projectionLength,
                    bendNode.Position,
                    directionToBoundary
                )
                {
                    Name = $"Generated segment for {bendProxy.Name}"
                };
                result.Add(newSegmentProxy);
            }

            return result;
        }
    }
}