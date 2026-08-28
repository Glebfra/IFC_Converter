using System;
using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.BoundaryResolvers
{
    internal sealed class PipeSegmentBoundaryResolver : IBoundaryResolver
    {
        public IEnumerable<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies)
        {
            return ResolveBoundary(proxy);
        }

        public IReadOnlyCollection<Vector<double>> ResolveBoundary(IEntityProxy proxy)
        {
            if (!(proxy is PipeSegmentProxy pipeSegmentProxy))
                throw new InvalidCastException();

            return new[]
            {
                pipeSegmentProxy.Position, pipeSegmentProxy.EndPosition
            };
        }
    }
}