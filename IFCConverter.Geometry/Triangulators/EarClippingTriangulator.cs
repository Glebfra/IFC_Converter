using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Triangulators
{
    internal class EarClippingTriangulator : ITriangulator
    {
        public int[][] Triangulate(FixedVector<Dim3>[] vertices)
        {
            FixedVector<Dim3>[] verticesArr = vertices ?? vertices.ToArray();
            PlaneProjection projection = PlaneProjectionFactory.Create(verticesArr);
            FixedVector<Dim2>[] points2D = projection.Project(verticesArr);

            if (points2D.Length < 3)
                throw new ArgumentException("Polygon must contain at least 3 vertices");

            List<int[]> triangles = new List<int[]>();
            List<int> polygon = new List<int>(points2D.Length);

            for (int i = 0; i < vertices.Length; i++)
            {
                polygon.Add(i);
            }

            if (!IsCounterClockwise(points2D))
                polygon.Reverse();

            while (polygon.Count > 3)
            {
                bool earFound = false;

                for (int i = 0; i < polygon.Count; i++)
                {
                    int prev = polygon[(i - 1 + polygon.Count) % polygon.Count];
                    int curr = polygon[i];
                    int next = polygon[(i + 1) % polygon.Count];

                    if (!IsConvex(points2D[prev], points2D[curr], points2D[next]))
                        continue;

                    bool containsPoint = false;

                    for (int j = 0; j < polygon.Count; j++)
                    {
                        int p = polygon[j];

                        if (p == prev || p == curr || p == next)
                            continue;

                        if (PointInTriangle(points2D[p], points2D[prev], points2D[curr], points2D[next]))
                        {
                            containsPoint = true;
                            break;
                        }
                    }

                    if (containsPoint)
                        continue;

                    triangles.Add(new[]
                    {
                        prev, curr, next
                    });
                    polygon.RemoveAt(i);

                    earFound = true;
                    break;
                }

                if (!earFound)
                    throw new InvalidOperationException("Failed to triangulate polygon. Polygon may be self-intersecting");
            }

            triangles.Add(new[]
            {
                polygon[0], polygon[1], polygon[2]
            });
            return triangles.ToArray();
        }

        private bool IsCounterClockwise(IReadOnlyList<FixedVector<Dim2>> vertices)
        {
            double area = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                FixedVector<Dim2> a = vertices[i];
                FixedVector<Dim2> b = vertices[(i + 1) % vertices.Count];

                area += a.GetX() * b.GetY() - b.GetX() * a.GetY();
            }

            return area > 0;
        }

        private double Cross(FixedVector<Dim2> a, FixedVector<Dim2> b, FixedVector<Dim2> c)
        {
            return (b.GetX() - a.GetX()) * (c.GetY() - a.GetY()) -
                   (b.GetY() - a.GetY()) * (c.GetX() - a.GetX());
        }

        private bool IsConvex(FixedVector<Dim2> a, FixedVector<Dim2> b, FixedVector<Dim2> c)
        {
            return Cross(a, b, c) > 0;
        }

        private bool PointInTriangle(FixedVector<Dim2> p, FixedVector<Dim2> a, FixedVector<Dim2> b, FixedVector<Dim2> c)
        {
            double c1 = Cross(a, b, p);
            double c2 = Cross(b, c, p);
            double c3 = Cross(c, a, p);

            bool hasNegative = c1 < 0 || c2 < 0 || c3 < 0;
            bool hasPositive = c1 > 0 || c2 > 0 || c3 > 0;

            return !(hasNegative && hasPositive);
        }
    }

    internal static class PlaneProjectionFactory
    {
        public static PlaneProjection Create(FixedVector<Dim3>[] points)
        {
            if (points.Length < 3)
                throw new ArgumentException("At least 3 points are required");

            FixedVector<Dim3> origin = points[0];
            FixedVector<Dim3> u = (points[1] - origin).Normalize();

            FixedVector<Dim3> normal = (points[1] - origin).CrossProduct(points[2] - origin).Normalize();
            FixedVector<Dim3> v = normal.CrossProduct(u);

            return new PlaneProjection(origin, u, v);
        }
    }

    internal class PlaneProjection
    {
        private readonly FixedVector<Dim3> _origin;
        private readonly FixedVector<Dim3> _u;
        private readonly FixedVector<Dim3> _v;

        public PlaneProjection(FixedVector<Dim3> origin, FixedVector<Dim3> u, FixedVector<Dim3> v)
        {
            _origin = origin;
            _u = u;
            _v = v;
        }

        public FixedVector<Dim2> Project(FixedVector<Dim3> point)
        {
            FixedVector<Dim3> d = point - _origin;
            return FixedVector<Dim2>.Builder.Dense(d.Dot(_u), d.Dot(_v));
        }

        public FixedVector<Dim2>[] Project(FixedVector<Dim3>[] points)
        {
            FixedVector<Dim2>[] result = new FixedVector<Dim2>[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                result[i] = Project(points[i]);
            }

            return result;
        }
    }
}