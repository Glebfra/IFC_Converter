using System;
using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.MeshResolvers
{
    internal sealed class PlanarComponentFinder
    {
        private readonly double _normalTolerance;

        public PlanarComponentFinder(double normalTolerance = 1e-6)
        {
            _normalTolerance = normalTolerance;
        }

        public IReadOnlyList<PlanarComponent> Find(IMesh mesh)
        {
            Dictionary<int, List<int>> adjacency = BuildTriangleAdjacency(mesh);
            bool[] visited = new bool[mesh.Triangles.Length];
            List<PlanarComponent> components = new List<PlanarComponent>();

            for (int start = 0; start < mesh.Triangles.Length; start++)
            {
                if (visited[start])
                    continue;

                FixedVector<Dim3> normal = mesh.Normals[start];

                if (normal.L2Norm() < 1e-12)
                {
                    visited[start] = true;
                    continue;
                }

                List<int> triangleIndices = new List<int>();

                Queue<int> queue = new Queue<int>();
                queue.Enqueue(start);
                visited[start] = true;

                while (queue.Count > 0)
                {
                    int current = queue.Dequeue();
                    triangleIndices.Add(current);

                    foreach (int neighbour in adjacency[current])
                    {
                        if (visited[neighbour])
                            continue;

                        double dot = Math.Abs(mesh.Normals[current].Dot(mesh.Normals[neighbour]));
                        if (dot < 1.0 - _normalTolerance)
                            continue;

                        visited[neighbour] = true;
                        queue.Enqueue(neighbour);
                    }
                }

                components.Add(new PlanarComponent(triangleIndices, normal, CalculateArea(mesh, triangleIndices)));
            }

            return components;
        }

        private static Dictionary<int, List<int>> BuildTriangleAdjacency(IMesh mesh)
        {
            Dictionary<(int, int), List<int>> edgeTriangles = new Dictionary<(int, int), List<int>>();
            for (int i = 0; i < mesh.Triangles.Length; i++)
            {
                int[] triangle = mesh.Triangles[i];
                AddEdge(edgeTriangles, triangle[0], triangle[1], i);
                AddEdge(edgeTriangles, triangle[1], triangle[2], i);
                AddEdge(edgeTriangles, triangle[2], triangle[0], i);
            }

            Dictionary<int, List<int>> adjacency = new Dictionary<int, List<int>>();
            for (int i = 0; i < mesh.Triangles.Length; i++)
                adjacency[i] = new List<int>();

            foreach (List<int> trianglesForEdge in edgeTriangles.Values)
            {
                for (int i = 0; i < trianglesForEdge.Count; i++)
                {
                    for (int j = i + 1; j < trianglesForEdge.Count; j++)
                    {
                        int a = trianglesForEdge[i];
                        int b = trianglesForEdge[j];

                        adjacency[a].Add(b);
                        adjacency[b].Add(a);
                    }
                }
            }

            return adjacency;
        }

        private static double CalculateArea(IMesh mesh, IReadOnlyList<int> triangleIndices)
        {
            double area = 0;

            foreach (int triangleIndex in triangleIndices)
            {
                int[] triangle = mesh.Triangles[triangleIndex];

                FixedVector<Dim3> p0 = mesh.Vertices[triangle[0]];
                FixedVector<Dim3> p1 = mesh.Vertices[triangle[1]];
                FixedVector<Dim3> p2 = mesh.Vertices[triangle[2]];

                FixedVector<Dim3> cross = (p1 - p0).CrossProduct(p2 - p0);

                area += cross.L2Norm();
            }

            return area * 0.5;
        }

        private static void AddEdge(Dictionary<(int, int), List<int>> edges, int a, int b, int triangleIndex)
        {
            (int, int) edge = a < b ? (a, b) : (b, a);
            if (!edges.TryGetValue(edge, out List<int> list))
            {
                list = new List<int>();
                edges[edge] = list;
            }

            list.Add(triangleIndex);
        }
    }
}