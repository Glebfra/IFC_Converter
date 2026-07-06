using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.BoundaryResolvers
{
    internal sealed class AttachmentBoundaryResolver : IBoundaryResolver
    {
        public IEnumerable<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies)
        {
            return new[]
            {
                proxy.Position
            };
        }
    }
}