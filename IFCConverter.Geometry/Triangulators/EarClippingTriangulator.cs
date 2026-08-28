using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Utils;

namespace IFCConverter.Geometry.Triangulators
{
    internal class EarClippingTriangulator : ITriangulator
    {
        public int[][] Triangulate(Vector<double>[] vertices)
        {
            PlaneProjection projection = PlaneProjectionFactory.Create(vertices);
            Vector2[] points2D = projection.Project(vertices);

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

                    triangles.Add(new int[]
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

            triangles.Add(new int[]
            {
                polygon[0], polygon[1], polygon[2]
            });
            return triangles.ToArray();
        }
        
        private static bool IsCounterClockwise(IReadOnlyList<Vector2> vertices)
        {
            double area = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i + 1) % vertices.Count];

                area += a.X * b.Y - b.X * a.Y;
            }

            return area > 0;
        }

        private static double Cross(Vector2 a, Vector2 b, Vector2 c)
        {
            return (b.X - a.X) * (c.Y - a.Y) -
                   (b.Y - a.Y) * (c.X - a.X);
        }

        private static bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
        {
            return Cross(a, b, c) > 0;
        }

        private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            double c1 =  Cross(a, b, p);
            double c2 = Cross(b, c, p);
            double c3 = Cross(c, a, p);

            bool hasNegative = c1 < 0 || c2 < 0 || c3 < 0;
            bool hasPositive = c1 > 0 || c2 > 0 || c3 > 0;
            
            return !(hasNegative && hasPositive);
        }
    }
    
    internal static class PlaneProjectionFactory
    {
        public static PlaneProjection Create(Vector<double>[] points)
        {
            if (points.Length < 3)
                throw new ArgumentException("At least 3 points are required");

            Vector<double> origin = points[0];
            Vector<double> u = (points[1] - origin).Normalize(2);

            Vector<double> normal = ((points[1] - origin).CrossProduct(points[2] - origin)).Normalize(2);
            Vector<double> v = normal.CrossProduct(u);
            
            return new PlaneProjection(origin, u, v);
        }
    }
    
    internal class PlaneProjection
    {
        private readonly Vector<double> _origin;
        private readonly Vector<double> _u;
        private readonly Vector<double> _v;

        public PlaneProjection(Vector<double> origin, Vector<double> u, Vector<double> v)
        {
            _origin = origin;
            _u = u;
            _v = v;
        }

        public Vector2 Project(Vector<double> point)
        {
            Vector<double> d = point - _origin;
            return new Vector2(
                d.DotProduct(_u),
                d.DotProduct(_v)
            );
        }

        public Vector2[] Project(Vector<double>[] points)
        {
            Vector2[] result = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                result[i] = Project(points[i]);
            }

            return result;
        }
    }
    
    internal struct Vector2
    {
        public double X;
        public double Y;

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector<double> ToVector2()
        {
            return new DenseVector(new double[]
            {
                X, Y
            });
        }

        public Vector<double> ToVector3()
        {
            return new DenseVector(new double[]
            {
                X, Y, 0.0
            });
        }
    }   
}