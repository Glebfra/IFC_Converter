using System;
using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.BoundaryResolvers
{
    internal sealed class TeeBoundaryResolver : IBoundaryResolver
    {
        public IEnumerable<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies)
        {
            if (proxy is not TeeProxy teeProxy)
                throw new InvalidCastException();

            return new[]
            {
                teeProxy.Position + teeProxy.HeadProjection, teeProxy.Position + teeProxy.MainProjection / 2,
                teeProxy.Position - teeProxy.MainProjection / 2
            };
        }
    }
}