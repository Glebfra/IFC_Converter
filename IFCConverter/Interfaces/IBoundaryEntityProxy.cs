using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Interfaces
{
    public interface IBoundaryEntityProxy : IEntityProxy
    {
        [Pure]
        public IEnumerable<Vector<double>> GetBoundaryPoints();
    }
}