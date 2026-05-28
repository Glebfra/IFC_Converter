using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.ConnectionResolvers
{
    internal sealed class BoundPointConnectionResolver : IEntityConnectionResolver
    {
        private readonly VectorComparer _comparer;

        public BoundPointConnectionResolver(VectorComparer comparer)
        {
            _comparer = comparer;
        }

        public IEnumerable<IEntityProxy> GetConnectedEntities(
            IEntityProxy proxy,
            IReadOnlyCollection<IEntityProxy> allProxies,
            int? count = null)
        {
            HashSet<IEntityProxy> result = new();
            IReadOnlyCollection<Vector<double>> entityPoints = proxy.Boundary.ToArray();

            foreach (IEntityProxy candidate in allProxies)
            {
                if (ReferenceEquals(proxy, candidate))
                    continue;

                IReadOnlyCollection<Vector<double>> candidatePoints = candidate.Boundary.ToArray();
                bool isConnected = entityPoints.Any(p1 =>
                    candidatePoints.Any(p2 => _comparer.Equals(p1, p2))
                );

                if (isConnected && (count == null || result.Count < count))
                    result.Add(candidate);
            }

            return result;
        }
    }
}