using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using Start.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IEntityProxy
    {
        public Vector<double> Position { get; }
        public IEnumerable<Vector<double>> Boundary { get; }
        public string? Name { get; set; }

        [Pure]
        public IStartEntity ToStartEntity();
    }
}