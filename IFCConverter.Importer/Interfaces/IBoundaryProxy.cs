using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IBoundaryProxy
    {
        public IEntityProxy Proxy { get; }
        public IReadOnlyCollection<Vector<double>> Boundary { get; }
    }
}