using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.ConnectionResolvers
{
    internal sealed class BoundPointConnectionResolver
    {
        private const double Tolerance = 1e-3;
        private readonly VectorComparer _comparer = new(Tolerance);

        public IEnumerable<IBoundaryProxy> GetConnectedEntities(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> allProxies)
        {
            HashSet<IBoundaryProxy> result = new();
            IReadOnlyCollection<Vector<double>> entityPoints = proxy.Boundary;

            foreach (IBoundaryProxy candidate in allProxies)
            {
                if (ReferenceEquals(proxy, candidate))
                    continue;

                IReadOnlyCollection<Vector<double>> candidatePoints = candidate.Boundary;
                bool isConnected = entityPoints.Any(p1 =>
                    candidatePoints.Any(p2 => _comparer.Equals(p1, p2))
                );
                if (isConnected)
                    result.Add(candidate);
            }

            return result;
        }
    }
}