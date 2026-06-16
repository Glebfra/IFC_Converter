using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Entities.Proxies
{
    internal sealed class BoundaryProxy : IBoundaryProxy
    {
        public BoundaryProxy(IEntityProxy proxy, IReadOnlyCollection<Vector<double>> boundary)
        {
            Proxy = proxy;
            Boundary = boundary;
        }

        public IEntityProxy Proxy { get; }
        public IReadOnlyCollection<Vector<double>> Boundary { get; }
    }
}