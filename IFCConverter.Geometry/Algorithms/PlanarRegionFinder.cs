using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Geometry.Mesh;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Algorithms
{
    public static class PlanarRegionFinder
    {
        private readonly struct TriangleData
        {
            public FixedVector<Dim3> A { get; }
            public FixedVector<Dim3> B { get; }
            public FixedVector<Dim3> C { get; }
            
            public FixedVector<Dim3> Normal { get; }
            public FixedVector<Dim3> Centroid { get; }
            public double Area { get; }
            
            public TriangleData(FixedVector<Dim3> a, FixedVector<Dim3> b, FixedVector<Dim3> c)
            {
                A = a;
                B = b;
                C = c;

                FixedVector<Dim3> cross = (b - a).CrossProduct(c - a);
                double doubleArea = cross.L2Norm();

                Area = doubleArea * 0.5;
                Normal = cross.SafeNormalize();
                Centroid = (a + b + c) * (1.0 / 3.0);
            }
        }
        
        public static IReadOnlyList<PlanarRegion> Find(IMesh mesh, double distanceTolerance = 1e-3, double normalAngleToleranceDegrees = 3.0)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            if (distanceTolerance <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(distanceTolerance));

            if (normalAngleToleranceDegrees <= 0.0 || normalAngleToleranceDegrees >= 90.0)
                throw new ArgumentOutOfRangeException(nameof(normalAngleToleranceDegrees));

            int triangleCount = mesh.TriangleCount;

            TriangleData[] triangles = new TriangleData[triangleCount];
            Dictionary<Edge, List<int>> edgeToTriangles = new Dictionary<Edge, List<int>>();

            for (int i = 0; i < triangleCount; i++)
            {
                Triangle triangle = mesh.Triangles[i];
                TriangleData data = new TriangleData(mesh.Vertices[triangle.A], mesh.Vertices[triangle.B], mesh.Vertices[triangle.C]);

                triangles[i] = data;
                
                foreach (Edge edge in triangle.GetEdges())
                {
                    if (!edgeToTriangles.TryGetValue(edge, out List<int> adjacent))
                    {
                        adjacent = new List<int>();
                        edgeToTriangles.Add(edge, adjacent);
                    }

                    adjacent.Add(i);
                }
            }

            List<int>[] adjacency = new List<int>[triangleCount];
            for (int i = 0; i < triangleCount; i++)
                adjacency[i] = new List<int>();
            
            foreach (List<int> adjacent in edgeToTriangles.Values)
            {
                for (int i = 0; i < adjacent.Count; i++)
                {
                    for (int j = i + 1; j < adjacent.Count; j++)
                    {
                        adjacency[adjacent[i]].Add(adjacent[j]);
                        adjacency[adjacent[j]].Add(adjacent[i]);
                    }
                }
            }

            double minNormalDot = Math.Cos(normalAngleToleranceDegrees * Math.PI / 180.0);
            bool[] visited = new bool[triangleCount];
            List<PlanarRegion> regions = new List<PlanarRegion>();

            for (int seed = 0; seed < triangleCount; seed++)
            {
                if (visited[seed])
                    continue;

                TriangleData seedData = triangles[seed];
                if (seedData.Area < 1e-15)
                {
                    visited[seed] = true;
                    continue;
                }

                List<int> regionTriangles = new List<int>();
                Queue<int> queue = new Queue<int>();

                visited[seed] = true;
                queue.Enqueue(seed);

                while (queue.Count > 0)
                {
                    int current = queue.Dequeue();
                    regionTriangles.Add(current);
                    
                    foreach (int neighbour in adjacency[current])
                    {
                        if (visited[neighbour])
                            continue;

                        if (!IsCompatible(seedData, triangles[neighbour], distanceTolerance, minNormalDot)) 
                            continue;

                        visited[neighbour] = true;
                        queue.Enqueue(neighbour);
                    }
                }
                
                if (regionTriangles.Count == 0)
                    continue;

                PlanarRegion region = BuildRegion(mesh, triangles, regionTriangles, distanceTolerance);
                if (region.TriangleCount < 3)
                    continue;
                
                regions.Add(region);
            }

            Rank(regions);

            return regions.OrderByDescending(region => region.Score).ToArray();
        }

        private static bool IsCompatible(TriangleData plane, TriangleData candidate, double distanceTolerance, double minNormalDot)
        {
            if (candidate.Area <= 1e-15)
                return false;

            double normalDot = Math.Abs(plane.Normal.Dot(candidate.Normal));
            if (normalDot < minNormalDot)
                return false;
            
            return DistanceToPlane(plane, candidate.A) <= distanceTolerance && 
                   DistanceToPlane(plane, candidate.B) <= distanceTolerance && 
                   DistanceToPlane(plane, candidate.C) <= distanceTolerance;
        }

        private static double DistanceToPlane(TriangleData plane, FixedVector<Dim3> point)
        {
            return Math.Abs(plane.Normal.Dot(point - plane.A));
        }

        private static PlanarRegion BuildRegion(IMesh mesh, TriangleData[] triangles, List<int> triangleIndices, double distanceTolerance)
        {
            Dictionary<Edge, int> edgeUsage = new Dictionary<Edge, int>();
            double area = 0;
            
            foreach (int index in triangleIndices)
            {
                Triangle triangle = mesh.Triangles[index];
                area += triangles[index].Area;
                
                foreach (Edge edge in triangle.GetEdges())
                {
                    if (edgeUsage.TryGetValue(edge, out int count))
                        edgeUsage[edge] = count + 1;
                    else
                        edgeUsage.Add(edge, 1);
                }
            }

            Edge[] boundaryEdges = edgeUsage.Where(pair => pair.Value == 1).Select(pair => pair.Key).ToArray();
            double perimeter = boundaryEdges.Sum(edge =>
            {
                FixedVector<Dim3> a = mesh.Vertices[edge.A];
                FixedVector<Dim3> b = mesh.Vertices[edge.B];
                return (b - a).L2Norm();
            });

            TriangleData seed = triangles[triangleIndices[0]];

            double sumSquaredDeviation = 0;
            double maxDeviation = 0;
            int vertexSamples = 0;
            
            foreach (int index in triangleIndices)
            {
                TriangleData triangle = triangles[index];
                FixedVector<Dim3>[] vertices = new FixedVector<Dim3>[]{ triangle.A, triangle.B, triangle.C };
                
                foreach (FixedVector<Dim3> vertex in vertices)
                {
                    double deviation = DistanceToPlane(seed, vertex);
                    maxDeviation = Math.Max(maxDeviation, deviation);
                    sumSquaredDeviation += deviation * deviation;
                    vertexSamples++;
                }
            }

            double rmsDeviation = vertexSamples == 0 ? 0 : Math.Sqrt(sumSquaredDeviation / vertexSamples);
            double compactness = perimeter <= 1e-15 ? 0 : 4.0 * Math.PI * area / (perimeter * perimeter);

            return new PlanarRegion(triangleIndices.ToArray(), boundaryEdges, area, perimeter, maxDeviation, rmsDeviation, compactness);
        }

        private static void Rank(List<PlanarRegion> regions)
        {
            if (regions.Count == 0)
                return;

            double maxArea = regions.Max(region => region.Area);
            int maxTriangleCount = regions.Max(region => region.TriangleCount);
            
            foreach (PlanarRegion region in regions)
            {
                double areaScore = maxArea > 0 ? Math.Sqrt(region.Area / maxArea) : 0;
                double triangleScore = maxTriangleCount > 0 ? Math.Sqrt((double)region.TriangleCount / maxTriangleCount) : 0;
                double compactnessScore = Math.Max(0, Math.Min(1, region.Compactness));
                double planarityScore = 1.0 / (1.0 + region.RmsDeviation);

                region.Score = 0.35 * areaScore +
                               0.25 * triangleScore +
                               0.25 * compactnessScore +
                               0.15 * planarityScore;
            }
        }
    }
}