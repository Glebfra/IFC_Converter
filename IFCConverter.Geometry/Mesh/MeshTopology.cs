using System.Collections.Generic;
using System.Linq;

namespace IFCConverter.Geometry.Mesh
{
    public static class MeshTopology
    {
        public static IEnumerable<Edge> GetBoundaryEdges(IEnumerable<Triangle> triangles)
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

            return edgeUsage
                .Where(pair => pair.Value == 1)
                .Select(pair => pair.Key);
        }

        public static IEnumerable<Edge> GetBoundaryEdges(IMesh mesh)
        {
            return GetBoundaryEdges(mesh.GetTriangles());
        }
    }
}