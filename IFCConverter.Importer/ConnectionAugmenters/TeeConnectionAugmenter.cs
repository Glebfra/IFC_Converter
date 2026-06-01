using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.ConnectionAugmenters
{
    internal sealed class TeeConnectionAugmenter : IEntityConnectionAugmenter
    {
        private const double DoubleEpsilon = 1e-3;
        private readonly VectorComparer _comparer = new VectorComparer(DoubleEpsilon);
        
        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity topology)
        {
            List<ISegmentProxy> generatedSegments = new List<ISegmentProxy>();
            
            if (topology.Proxy is not TeeProxy teeProxy)
                throw new Exception($"{nameof(topology.Proxy)} should be {nameof(BendProxy)}");
            
            foreach (Vector<double> boundaryPoint in teeProxy.Boundary)
            {
                bool isConnected = topology.ConnectedProxies
                    .OfType<ISegmentProxy>()
                    .Any(segment => _comparer.Equals(segment.Position, boundaryPoint) ||
                                    _comparer.Equals(segment.EndPosition, boundaryPoint));
                
                if (isConnected)
                    continue;

                Vector<double> projection = boundaryPoint - teeProxy.Position;
                double length = projection.L2Norm();
                Vector<double> direction = projection / length;
                double diameter = direction.IsParallel(teeProxy.MainProjection)
                    ? teeProxy.MainDiameter
                    : teeProxy.HeadDiameter;

                PipeSegmentProxy generatedSegment = new PipeSegmentProxy(
                    diameter,
                    length,
                    teeProxy.Position,
                    direction
                )
                {
                    Name = $"Generated segment for {teeProxy.Name}"
                };
            }

            return generatedSegments;
        }
    }
}