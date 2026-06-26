using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;
using Utils;
using VectorExtensions = Utils.VectorExtensions;

namespace IFCConverter.Importer.ConnectionAugmenters
{
    internal sealed class ReducerConnectionAugmenter : IEntityConnectionAugmenter
    {
        private const double MinLength = 1e-2;
        private const double DoubleEpsilon = 1e-3;
        private readonly VectorComparer _comparer = new(DoubleEpsilon);

        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity topology)
        {
            List<ISegmentProxy> generatedSegments = new();

            if (topology.Proxy.Proxy is not ReducerProxy reducerProxy)
                throw new Exception($"{nameof(topology.Proxy)} should be {nameof(ReducerProxy)}");

            foreach (Vector<double> boundaryPoint in topology.Proxy.Boundary)
            {
                bool isConnected = topology.Connected
                    .OfType<SegmentTopologyEntity>()
                    .Any(segment => _comparer.Equals(segment.Nodes.ElementAt(0).Position, boundaryPoint) ||
                                    _comparer.Equals(segment.Nodes.ElementAt(1).Position, boundaryPoint));

                if (isConnected)
                    continue;

                double diameter = _comparer.Equals(boundaryPoint, reducerProxy.Position)
                    ? reducerProxy.MinDiameter
                    : reducerProxy.MaxDiameter;
                Vector<double> projection = boundaryPoint - reducerProxy.Position;
                if (_comparer.Equals(projection, VectorExtensions.Zero))
                {
                    projection = reducerProxy.Direction.Negate() * MinLength;
                }

                double length = projection.L2Norm();
                Vector<double> direction = projection.Normalize(2);

                PipeSegmentProxy virtualSegment = new(
                    diameter,
                    length,
                    reducerProxy.Position,
                    direction
                )
                {
                    Name = $"Generated segment for {reducerProxy.Name}"
                };

                generatedSegments.Add(virtualSegment);
            }

            return generatedSegments;
        }
    }
}