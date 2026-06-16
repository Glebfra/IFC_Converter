using System;
using System.Collections.Generic;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.BoundaryResolvers
{
    internal sealed class ReducerBoundaryResolver : IBoundaryResolver
    {
        public IReadOnlyCollection<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies, int boundaryCount)
        {
            if (proxy is not ReducerProxy reducerProxy)
                throw new InvalidCastException();

            return reducerProxy.Boundary;
        }
    }
}