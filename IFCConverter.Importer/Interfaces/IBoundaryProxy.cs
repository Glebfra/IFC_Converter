using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Start.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IBoundaryProxy
    {
        public IEntityProxy Proxy { get; set; }
        public IReadOnlyCollection<Vector<double>> Boundary { get; set; }
        public IStartEntity ToStartEntity();
    }
}