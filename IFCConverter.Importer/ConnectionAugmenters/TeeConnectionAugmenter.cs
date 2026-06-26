using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.ConnectionAugmenters
{
    internal sealed class TeeConnectionAugmenter : IEntityConnectionAugmenter
    {
        private const double MinLength = 1e-2;
        private const double DoubleEpsilon = 1e-3;
        private readonly VectorComparer _comparer = new(DoubleEpsilon);

        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity topology)
        {
            List<ISegmentProxy> generatedSegments = new();

            if (topology.Proxy.Proxy is not TeeProxy teeProxy)
                throw new Exception($"{nameof(topology.Proxy)} should be {nameof(TeeProxy)}");

            foreach (Vector<double> boundaryPoint in topology.Proxy.Boundary)
            {
                bool isConnected = topology.Connected
                    .OfType<SegmentTopologyEntity>()
                    .Any(segment => _comparer.Equals(segment.Nodes.ElementAt(0).Position, boundaryPoint) ||
                                    _comparer.Equals(segment.Nodes.ElementAt(1).Position, boundaryPoint));

                if (isConnected)
                    continue;

                Vector<double> projection = boundaryPoint - teeProxy.Position;
                double length = projection.L2Norm();
                if (length < MinLength)
                    length = MinLength;
                Vector<double> direction = projection / length;
                double diameter = direction.IsParallel(teeProxy.MainProjection)
                    ? teeProxy.MainDiameter
                    : teeProxy.HeadDiameter;

                PipeSegmentProxy generatedSegment = new(
                    diameter,
                    length,
                    teeProxy.Position,
                    direction
                )
                {
                    Name = $"Generated segment for {teeProxy.Name}"
                };
                generatedSegments.Add(generatedSegment);
            }

            return generatedSegments;
        }
    }
}