using System.Collections.Generic;
using System.Linq;
using IFCConverter.Geometry.Mesh;

namespace IFCConverter.Geometry.Boundary
{
    public sealed class BoundaryLoop
    {
        private List<int> _vertices;
        
        public IReadOnlyList<int> Vertices => _vertices;
        public int Count => _vertices.Count;

        public BoundaryLoop(IEnumerable<int> vertices)
        {
            _vertices = vertices.ToList();
        }

        public IEnumerable<Edge> GetEdges()
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                int a = _vertices[i];
                int b = _vertices[(i + 1) % _vertices.Count];

                yield return new Edge(a, b);
            }
        }
    }
}