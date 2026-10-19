using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Geometry.Mesh;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Algorithms
{
    public static class ClosedMeshTopology
    {
        private const double DoubleTolerance = 1e-6;
        private static readonly VectorComparer Comparer = new VectorComparer(DoubleTolerance);
        
        public static void AnalyzeEdgeUsage(IMesh mesh)
        {
            Dictionary<Edge, int> usage = new Dictionary<Edge, int>();

            foreach (Triangle triangle in mesh.Triangles)
            {
                foreach (Edge edge in triangle.GetEdges())
                {
                    if (usage.TryGetValue(edge, out int count))
                        usage[edge] = count + 1;
                    else
                        usage.Add(edge, 1);
                }
            }

            int boundary = usage.Count(pair => pair.Value == 1);
            int manifold = usage.Count(pair => pair.Value == 2);
            int nonManifold = usage.Count(pair => pair.Value > 2);

            Console.WriteLine($"Triangles: {mesh.Triangles.Count}");
            Console.WriteLine($"Boundary edges: {boundary}");
            Console.WriteLine($"Edges used twice: {manifold}");
            Console.WriteLine($"Non-manifold edges: {nonManifold}");
        }
        
        [Pure]
        public static IEnumerable<Edge> GetBoundaryEdges(IMesh mesh)
        {
            Dictionary<FixedVector<Dim3>, List<int>> normalTriangles = GetNormalTriangles(mesh);
            IReadOnlyCollection<int> indices = normalTriangles.Where(pair => pair.Value.Count > 2).SelectMany(pair => pair.Value).ToArray();
            return GetBoundaryEdges(mesh, indices);
        }

        [Pure]
        private static IEnumerable<Edge> GetBoundaryEdges(IMesh mesh, IReadOnlyCollection<int> triangleIndices)
        {
            Dictionary<Edge, int> edgeUsage = new Dictionary<Edge, int>();

            foreach (int triangleIndex in triangleIndices)
            {
                foreach (Edge edge in mesh.Triangles[triangleIndex].GetEdges())
                {
                    if (edgeUsage.TryGetValue(edge, out int count))
                        edgeUsage[edge] = count + 1;
                    else 
                        edgeUsage.Add(edge, 1);
                }
            }

            return edgeUsage.Where(pair => pair.Value == 1).Select(pair => pair.Key);
        }

        [Pure]
        private static Dictionary<FixedVector<Dim3>, List<int>> GetNormalTriangles(IMesh mesh)
        {
            Dictionary<FixedVector<Dim3>, List<int>> result = new Dictionary<FixedVector<Dim3>, List<int>>(Comparer);

            for (int index = 0; index < mesh.FaceNormalCount; index++)
            {
                FixedVector<Dim3> normal = mesh.FaceNormals[index];

                if (!result.TryGetValue(normal, out List<int> triangles))
                {
                    triangles = new List<int>();
                    result.Add(normal, triangles);
                }
                triangles.Add(index);
            }

            return result;
        }
    }
}