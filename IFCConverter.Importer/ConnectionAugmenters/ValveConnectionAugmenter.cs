using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Interfaces;
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
            List<ISegmentProxy> generatedSegments = new List<ISegmentProxy>();
            
            if (topology is not ValveTopologyEntity valveTopologyEntity)
                throw new Exception($"{nameof(topology)} should be {nameof(ValveTopologyEntity)}");

            Vector<double> position = topology.Proxy.Proxy.Position;
            IReadOnlyCollection<ISegmentProxy> connectedSegments = topology.ConnectedProxies
                .Select(proxy => proxy.Proxy)
                .OfType<ISegmentProxy>()
                .ToArray();
            double diameter = connectedSegments.Max(segment => segment.Diameter);
            
            foreach (Vector<double> boundary in topology.Proxy.Boundary)
            {
                bool isConnected = connectedSegments
                    .Any(segment => _comparer.Equals(segment.Position, boundary) ||
                                    _comparer.Equals(segment.EndPosition, boundary));
                
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