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
    internal sealed class ValveConnectionAugmenter : IEntityConnectionAugmenter
    {
        private const double DoubleEpsilon = 1e-3;
        private readonly VectorComparer _comparer = new(DoubleEpsilon);

        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity topology)
        {
            List<ISegmentProxy> generatedSegments = new();

            if (topology is not ValveTopologyEntity valveTopologyEntity)
                throw new Exception($"{nameof(topology)} should be {nameof(ValveTopologyEntity)}");

            Vector<double> position = topology.Proxy.Proxy.Position;
            IReadOnlyCollection<ISegmentProxy> connectedSegments = topology.Connected
                .Select(obj => obj.Proxy.Proxy)
                .OfType<ISegmentProxy>()
                .ToArray();
            double diameter = connectedSegments.Max(segment => segment.Diameter);

            foreach (Vector<double> boundary in topology.Proxy.Boundary)
            {
                bool isConnected = topology.Connected
                    .OfType<SegmentTopologyEntity>()
                    .Any(segment => _comparer.Equals(segment.Nodes.ElementAt(0).Position, boundary) ||
                                    _comparer.Equals(segment.Nodes.ElementAt(1).Position, boundary));

                if (isConnected)
                    continue;

                Vector<double> projection = boundary - position;
                double length = projection.L2Norm();
                Vector<double> direction = projection / length;

                PipeSegmentProxy generatedSegment = new(
                    diameter,
                    length,
                    position,
                    direction
                )
                {
                    Name = $"Generated segment for {topology.Proxy.Proxy.Name}"
                };
                generatedSegments.Add(generatedSegment);
            }

            return generatedSegments;
        }
    }
}