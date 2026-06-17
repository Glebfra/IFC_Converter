using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.ConnectionAugmenters
{
    internal sealed class BendConnectionAugmenter : IEntityConnectionAugmenter
    {
        private const double MinLength = 1e-2;
        private const double DoubleEpsilon = 1e-3;
        private readonly VectorComparer _comparer = new(DoubleEpsilon);

        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity topology)
        {
            List<ISegmentProxy> generatedSegments = new();

            if (topology.Proxy.Proxy is not BendProxy bendProxy)
                throw new Exception($"{nameof(topology.Proxy)} should be {nameof(BendProxy)}");

            foreach (Vector<double> boundaryPoint in topology.Proxy.Boundary)
            {
                bool isConnected = topology.ConnectedProxies
                    .Select(proxy => proxy.Proxy)
                    .OfType<ISegmentProxy>()
                    .Any(segment => _comparer.Equals(segment.Position, boundaryPoint) ||
                                    _comparer.Equals(segment.EndPosition, boundaryPoint));

                if (isConnected)
                    continue;

                double diameter = bendProxy.Diameter;
                Vector<double> projection = boundaryPoint - bendProxy.Position;
                double length = projection.L2Norm();
                if (length < MinLength)
                    length = MinLength;

                Vector<double> direction = projection / length;

                PipeSegmentProxy generatedSegment = new(
                    diameter,
                    length,
                    bendProxy.Position,
                    direction
                )
                {
                    Name = $"Generated segment for {bendProxy.Name}"
                };
                generatedSegments.Add(generatedSegment);
            }

            return generatedSegments;
        }
    }
}