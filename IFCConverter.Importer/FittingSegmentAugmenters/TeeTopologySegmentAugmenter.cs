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
    internal sealed class TeeTopologySegmentAugmenter : ITopologySegmentAugmenter
    {
        private const double DoubleTolerance = 1e-3;
        private static readonly VectorComparer Comparer = new(DoubleTolerance);
        
        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity entity)
        {
            if (entity is not TeeTopologyEntity teeTopologyEntity)
                throw new InvalidOperationException($"{nameof(entity)}  must be of type {nameof(TeeTopologyEntity)}");
            
            return Augment(teeTopologyEntity);
        }

        public IEnumerable<ISegmentProxy> Augment(TeeTopologyEntity entity)
        {
            IReadOnlyCollection<SegmentTopologyEntity> connectedSegments = entity.Connected.OfType<SegmentTopologyEntity>().ToArray();
            AugmentConnectedSegments(connectedSegments, entity);
            
            IEnumerable<ISegmentProxy> result = GenerateConnectedSegments(connectedSegments, entity);
            return result;
        }

        private static void AugmentConnectedSegments(IEnumerable<SegmentTopologyEntity> connectedSegments, TeeTopologyEntity teeTopologyEntity)
        {
            ITopologyNodeEntity teeNode = teeTopologyEntity.Node;
            foreach (SegmentTopologyEntity connectedSegment in connectedSegments)
            {
                ITopologyNodeEntity startNode = connectedSegment.Nodes.ElementAt(0);
                ITopologyNodeEntity endNode = connectedSegment.Nodes.ElementAt(1);
                
                if (Comparer.NearerThan(startNode.Position, endNode.Position, teeNode.Position))
                {
                    startNode = teeNode;
                }
                else
                {
                    endNode = teeNode;
                }
                
                Vector<double> projection = endNode.Position - startNode.Position;
                connectedSegment.Augment(startNode, endNode, projection);
            }
        }

        private static IEnumerable<ISegmentProxy> GenerateConnectedSegments(
            IEnumerable<SegmentTopologyEntity> existingSegments,
            TeeTopologyEntity teeTopologyEntity)
        {
            TeeProxy teeProxy = (TeeProxy)teeTopologyEntity.Proxy.Proxy;
            List<ISegmentProxy> result = new List<ISegmentProxy>();

            ITopologyNodeEntity teeNode = teeTopologyEntity.Node;
            IReadOnlyCollection<Vector<double>> segmentDirections = existingSegments.Select(segment => segment.Projection.Normalize(2)).ToArray();
            IReadOnlyCollection<Vector<double>> boundaries = teeTopologyEntity.Proxy.Boundary;
            foreach (Vector<double> boundary in boundaries)
            {
                Vector<double> projectionToBoundary = boundary - teeNode.Position;
                double projectionLength = projectionToBoundary.L2Norm();
                Vector<double> directionToBoundary = projectionToBoundary / projectionLength;
                bool isAlreadyConnected = segmentDirections.Any(direction => direction.IsParallel(directionToBoundary, DoubleTolerance));
                if (isAlreadyConnected)
                    continue;

                double diameter = directionToBoundary.IsParallel(teeProxy.MainProjection) 
                    ? teeProxy.MainDiameter 
                    : teeProxy.HeadDiameter;
                
                
                ISegmentProxy newSegmentProxy = new PipeSegmentProxy(
                    diameter,
                    projectionLength,
                    teeNode.Position,
                    directionToBoundary
                )
                {
                    Name = $"Generated segment for {teeProxy.Name}"
                };
                result.Add(newSegmentProxy);
            }
            
            return result;
        }
    }
}