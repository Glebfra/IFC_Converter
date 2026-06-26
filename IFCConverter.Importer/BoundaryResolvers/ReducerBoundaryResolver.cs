using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.BoundaryResolvers
{
    internal sealed class ReducerBoundaryResolver : IBoundaryResolver
    {
        public IEnumerable<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies)
        {
            if (proxy is not ReducerProxy reducerProxy)
                throw new InvalidCastException();

            return reducerProxy.Boundary.ToArray();
        }
    }
}