using System;
using System.Collections.Generic;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.BoundaryResolvers
{
    internal sealed class PipeSegmentBoundaryResolver : IBoundaryResolver
    {
        public IReadOnlyCollection<Vector<double>> ResolveBoundary(IEntityProxy proxy)
        {
            if (proxy is not PipeSegmentProxy pipeSegmentProxy)
                throw new InvalidCastException();
            
            return new Vector<double>[] { pipeSegmentProxy.Position, pipeSegmentProxy.EndPosition };
        }
        
        public IReadOnlyCollection<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies, int boundaryCount)
        {
            return ResolveBoundary(proxy);
        }
    }
}