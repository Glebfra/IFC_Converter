using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.BoundaryResolvers
{
    internal sealed class BoundaryResolver
    {
        private static readonly Lazy<BoundaryResolver> Instance = new Lazy<BoundaryResolver>(() => new BoundaryResolver());

        private BoundaryResolver()
        {
        }

        public static BoundaryResolver GetInstance()
        {
            return Instance.Value;
        }

        public IReadOnlyCollection<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies)
        {
            ProxyEntityAttribute attribute = proxy.GetProxyEntityAttribute();
            IBoundaryResolver resolver = attribute.GetBoundaryResolver();
            if (resolver == null)
                throw new InvalidOperationException($"Boundary resolver not configured for {proxy.GetType().Name}");

            return resolver.ResolveBoundary(proxy, allProxies).Take(attribute.ConnectionsCount).ToArray();
        }
    }
}