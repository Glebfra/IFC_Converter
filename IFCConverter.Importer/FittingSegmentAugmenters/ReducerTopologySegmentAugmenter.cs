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
    internal sealed class ReducerTopologySegmentAugmenter : ITopologySegmentAugmenter
    {
        private const double MinLength = 1e-2;
        private const double DoubleTolerance = 1e-3;
        private static readonly VectorComparer Comparer = new(DoubleTolerance);
        
        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity entity)
        {
            if (entity is not ReducerTopologyEntity reducerTopologyEntity)
                throw new InvalidOperationException($"{nameof(entity)}  must be of type {nameof(ReducerTopologyEntity)}");
            
            return Augment(reducerTopologyEntity);
        }

        public IEnumerable<ISegmentProxy> Augment(ReducerTopologyEntity entity)
        {
            IReadOnlyCollection<SegmentTopologyEntity> connectedSegments = entity.Connected.OfType<SegmentTopologyEntity>().ToArray();
            AugmentConnectedSegments(connectedSegments, entity);
            
            IEnumerable<ISegmentProxy> result = GenerateConnectedSegments(connectedSegments, entity);
            return result;
        }

        private static void AugmentConnectedSegments(IEnumerable<SegmentTopologyEntity> connectedSegments, ReducerTopologyEntity reducerTopologyEntity)
        {
            ITopologyNodeEntity reducerNode = reducerTopologyEntity.Node;
            
            foreach (SegmentTopologyEntity connectedSegment in connectedSegments)
            {
                ITopologyNodeEntity segmentStartNode = connectedSegment.Nodes.ElementAt(0);
                ITopologyNodeEntity segmentEndNode = connectedSegment.Nodes.ElementAt(1);

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
                connectedSegment.Augment(segmentStartNode, segmentEndNode, projection);
            }
        }

        private static IEnumerable<ISegmentProxy> GenerateConnectedSegments(
            IEnumerable<SegmentTopologyEntity> existingSegments,
            ReducerTopologyEntity reducerTopologyEntity)
        {
            IReadOnlyCollection<Vector<double>> boundaries = reducerTopologyEntity.Proxy.Boundary;
            ReducerProxy reducerProxy = (ReducerProxy)reducerTopologyEntity.Proxy.Proxy;
            List<ISegmentProxy> result = new List<ISegmentProxy>();
            Vector<double> reducerDirection = (boundaries.ElementAt(0) - boundaries.ElementAt(1) + reducerProxy.AxisDisplacement).Normalize(2);
            
            ITopologyNodeEntity reducerNode = reducerTopologyEntity.Node;
            foreach (Vector<double> boundary in boundaries)
            {
                Vector<double> projectionToBoundary = boundary - reducerNode.Position;
                double projectionLength = projectionToBoundary.L2Norm();
                if (projectionLength < MinLength)
                {
                    projectionLength = MinLength;
                    projectionToBoundary = reducerDirection.Negate() * projectionLength;
                }

                double diameter = boundary.Equals(reducerNode.Position)
                    ? reducerProxy.MinDiameter
                    : reducerProxy.MaxDiameter;
                
                Vector<double> directionToBoundary = projectionToBoundary / projectionLength;
                
                ISegmentProxy newSegmentProxy = new PipeSegmentProxy(
                    diameter,
                    projectionLength,
                    reducerNode.Position,
                    directionToBoundary
                )
                {
                    Name = $"Generated segment for {reducerProxy.Name}"
                };
                // result.Add(newSegmentProxy);
            }
            
            return result;
        }
    }
}