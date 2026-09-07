using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Interfaces
{
    public interface IBoundaryResolver
    {
        IEnumerable<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies);
    }
}