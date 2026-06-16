using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Interfaces
{
    public interface IBoundaryResolver
    {
        public IReadOnlyCollection<Vector<double>> ResolveBoundary(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies, int boundaryCount);
    }
}