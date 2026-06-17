using System;
using System.Collections.Generic;
using System.Reflection;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.BoundaryResolvers
{
    internal sealed class BoundaryResolver
    {
        private static readonly Lazy<BoundaryResolver> Instance = new(() => new BoundaryResolver());

        private BoundaryResolver()
        {
        }

        public static BoundaryResolver GetInstance()
        {
            return Instance.Value;
        }

        public IReadOnlyCollection<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies)
        {
            ProxyEntityAttribute attribute = proxy.GetType().GetCustomAttribute<ProxyEntityAttribute>();
            IBoundaryResolver? resolver = attribute.GetBoundaryResolver();
            if (resolver == null)
                throw new InvalidOperationException($"Boundary resolver not configured for {proxy.GetType().Name}");

            return resolver.ResolveBoundary(proxy, allProxies, attribute.ConnectionsCount);
        }
    }
}