using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Interfaces
{
    internal interface ITopologyProxy
    {
        [Pure]
        public IEnumerable<Vector<double>> GetBoundaryPoints();
    }
}