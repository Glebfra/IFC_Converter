using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Interfaces;

namespace IFCConverter.Converters.Importers.Proxies
{
    internal sealed class ReducerProxy : IFittingProxy
    {
        public Vector<double> Position { get; }
        
        public IEnumerable<Vector<double>> Boundary => _boundary ??= GetBoundaryPoints();
        private IEnumerable<Vector<double>>? _boundary;
        
        public ReducerProxy(Vector<double> position)
        {
            Position = position;
        }

        public IStartEntity ToStartEntity()
        {
            throw new System.NotImplementedException();
        }

        [Pure]
        private IEnumerable<Vector<double>> GetBoundaryPoints()
        {
            throw new System.NotImplementedException();
        }
    }
}