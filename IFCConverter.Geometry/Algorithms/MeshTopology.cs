using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace IFCConverter.Geometry.Algorithms
{
    public static class MeshTopology
    {
        [Pure]
        public static IEnumerable<Edge> GetBoundaryEdges(IMesh mesh)
        {
            return GetBoundaryEdges(mesh.Triangles);
        }

        [Pure]
        public static IDictionary<Edge, int> GetEdgeUsage(IMesh mesh)
        {
            return GetEdgeUsage(mesh.Triangles);
        }

        [Pure]
        public static IEnumerable<int> GetBoundaryVertices(IMesh mesh)
        {
            return GetBoundaryVertices(mesh.Triangles);
        }
        
        [Pure]
        private static IEnumerable<Edge> GetBoundaryEdges(IEnumerable<Triangle> triangles)
        {
            IDictionary<Edge, int> edgeUsage = GetEdgeUsage(triangles);
            return edgeUsage.Where(pair => pair.Value == 1).Select(pair => pair.Key);
        }
        
        [Pure]
        private static IDictionary<Edge, int> GetEdgeUsage(IEnumerable<Triangle> triangles)
        {
            Dictionary<Edge, int> edgeUsage = new Dictionary<Edge, int>();

            foreach (Triangle triangle in triangles)
            {
                foreach (Edge edge in triangle.GetEdges())
                {
                    if (edgeUsage.TryGetValue(edge, out int count))
                        edgeUsage[edge] = count + 1;
                    else 
                        edgeUsage.Add(edge, 1);
                }
            }

            return edgeUsage;
        }
        
        [Pure]
        private static IEnumerable<int> GetBoundaryVertices(IEnumerable<Triangle> triangles)
        {
            HashSet<int> vertices = new HashSet<int>();
            foreach (Edge edge in GetBoundaryEdges(triangles))
            {
                vertices.Add(edge.A);
                vertices.Add(edge.B);
            }

            return vertices;
        }
    }
}