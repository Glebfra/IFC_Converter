using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using Start.Interfaces;

namespace IFCConverter.Interfaces
{
    internal interface IEntityProxy
    {
        public IEnumerable<Vector<double>> Boundary { get; }
        
        [Pure]
        public IStartEntity ToStartEntity();
    }
}