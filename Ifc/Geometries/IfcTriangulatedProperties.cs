using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Utils;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace Ifc.Geometries
{
    public struct ConeTriangulatedGeometryProperties
    {
        public Vector<double> TopConePoint;
        public Vector<double> BottomConeCenter;
        public double Diameter;
    }

    public struct ClippedConeTriangulatedGeometryProperties
    {
        public Vector<double> TopConeCenter;
        public Vector<double> BottomConeCenter;
        public Vector<double>? Direction;
        public double TopDiameter;
        public double BottomDiameter;
    }

    public struct SphereTriangulatedGeometryProperties
    {
        public Vector<double> Center;
        public double Diameter;
    }

    public struct ExtrudedBodyTriangulatedGeometryProperties
    {
        public Vector<double>[] StartPoints;
        public Vector<double> ExtrudedDirection;
        public double Length;
    }

    public struct ExtrudedBodyByRefPointsTriangulatedGeometryProperties
    {
        public Vector<double>[] StartPoints;
        public Vector<double>[] RefPoints;
    }

    public struct PlaneTriangulatedGeometryProperties
    {
        public Vector<double>[] PolygonPoints;
    }

    public struct IfcTriangulatedProperties
    {
        public IEnumerable<Vector<double>> Coordinates;
        public IEnumerable<IEnumerable<int>> TriangleIndices;
        public IEnumerable<Vector<double>> Normals;

        private const int _numSegments = 16;

        [Pure]
        public static IfcTriangulatedProperties CreateSphere(SphereTriangulatedGeometryProperties properties)
        {
            double radius = properties.Diameter / 2;

            Vector<double>[] coordinates = new Vector<double>[_numSegments * _numSegments];
            Vector<double>[] normals = new Vector<double>[_numSegments * (_numSegments - 1) * 2];
            int[][] triangleIndices = new int[_numSegments * (_numSegments - 1) * 2][];

            for (int i = 0; i < _numSegments; i++)
            {
                double theta = Math.PI * i / (_numSegments - 1);
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int j = 0; j < _numSegments; j++)
                {
                    int index = i * _numSegments + j;
                    double phi = 2 * Math.PI * j / _numSegments;

                    double x = radius * sinTheta * Math.Cos(phi);
                    double y = radius * sinTheta * Math.Sin(phi);
                    double z = radius * cosTheta;
                    Vector<double> temp = new DenseVector(new[]
                    {
                        x, y, z
                    });

                    coordinates[index] = temp - properties.Center;
                }
            }

            int arrIndex = 0;
            for (int i = 0; i < _numSegments - 1; i++)
            for (int j = 0; j < _numSegments; j++)
            {
                int[] indexes =
                {
                    i * _numSegments + j, i * _numSegments + (j + 1) % _numSegments, (i + 1) * _numSegments + j,
                    (i + 1) * _numSegments + (j + 1) % _numSegments
                };

                triangleIndices[arrIndex] = new[]
                {
                    indexes[0] + 1, indexes[1] + 1, indexes[3] + 1
                };
                triangleIndices[arrIndex + 1] = new[]
                {
                    indexes[0] + 1, indexes[3] + 1, indexes[2] + 1
                };

                Vector<double> first = coordinates[triangleIndices[arrIndex][1] - 1]
                                       - coordinates[triangleIndices[arrIndex][0] - 1];
                Vector<double> second = coordinates[triangleIndices[arrIndex][2] - 1]
                                        - coordinates[triangleIndices[arrIndex][1] - 1];
                normals[arrIndex] = VectorExtensions.CreateNormalVector(first, second);

                first = coordinates[triangleIndices[arrIndex + 1][1] - 1]
                        - coordinates[triangleIndices[arrIndex + 1][0] - 1];
                second = coordinates[triangleIndices[arrIndex + 1][2] - 1]
                         - coordinates[triangleIndices[arrIndex + 1][1] - 1];
                normals[arrIndex + 1] = VectorExtensions.CreateNormalVector(first, second);

                arrIndex += 2;
            }

            return new IfcTriangulatedProperties
            {
                Coordinates = coordinates,
                Normals = normals,
                TriangleIndices = triangleIndices
            };
        }

        [Pure]
        public static IfcTriangulatedProperties CreateCone(ConeTriangulatedGeometryProperties properties)
        {
            Vector<double> heightDirection = properties.TopConePoint - properties.BottomConeCenter;
            Matrix<double> botMatrix = MatrixExtensions.CreateTransition(properties.BottomConeCenter, heightDirection);
            return CreateCone(properties, botMatrix, properties.TopConePoint);
        }

        [Pure]
        public static IfcTriangulatedProperties CreateClippedCone(ClippedConeTriangulatedGeometryProperties properties)
        {
            Vector<double> heightDirection = properties.Direction
                                             ?? properties.TopConeCenter - properties.BottomConeCenter;
            Vector<double> z = heightDirection.Normalize(2);
            Matrix<double> botMatrix = MatrixExtensions.CreateTransition(properties.BottomConeCenter, z);
            Matrix<double> topMatrix = MatrixExtensions.CreateTransition(properties.TopConeCenter, z);
            return CreateClippedCone(properties, botMatrix, topMatrix);
        }

        [Pure]
        public static IfcTriangulatedProperties CreateExtrudedBody(ExtrudedBodyTriangulatedGeometryProperties properties)
        {
            Vector<double>[] coordinates = new Vector<double>[properties.StartPoints.Length * 2];

            List<int[]> triangleIndices = new List<int[]>(properties.StartPoints.Length * 2);
            
            for (int i = 0; i < properties.StartPoints.Length; i++)
            {
                coordinates[i] = properties.StartPoints[i];
                coordinates[i + properties.StartPoints.Length] = properties.StartPoints[i] + properties.ExtrudedDirection * properties.Length;
            }

            Vector<double>[] startPolygon = coordinates.Take(properties.StartPoints.Length).ToArray();
            Vector<double>[] endPolygon = coordinates.Skip(properties.StartPoints.Length).ToArray();
            
            for (int i = 0; i < properties.StartPoints.Length; i++)
            {
                triangleIndices.Add(new int[]
                {
                    i + 1, i + properties.StartPoints.Length + 1, (i + 1) % properties.StartPoints.Length + properties.StartPoints.Length + 1 
                });
                triangleIndices.Add(new int[]
                {
                    i + 1, (i + 1) % properties.StartPoints.Length + properties.StartPoints.Length + 1, (i + 1) % properties.StartPoints.Length + 1
                });
            }
            
            int[][] startTriangleIndices = EarClippingTriangulator.Triangulate(startPolygon);
            int[][] endTriangleIndices = EarClippingTriangulator.Triangulate(endPolygon);

            for (int i = 0; i < startTriangleIndices.Length; i++)
            {
                for (int j = 0; j < startTriangleIndices[i].Length; j++)
                {
                    startTriangleIndices[i][j] += 1;
                }
            }

            for (int i = 0; i < endTriangleIndices.Length; i++)
            {
                for (int j = 0; j < endTriangleIndices[i].Length; j++)
                {
                    endTriangleIndices[i][j] += properties.StartPoints.Length + 1;
                }
            }
            
            triangleIndices.AddRange(startTriangleIndices);
            triangleIndices.AddRange(endTriangleIndices);

            int[][] triangleIndicesArr = triangleIndices.ToArray();

            Vector<double>[] normals = CreateNormals(coordinates, triangleIndicesArr);

            return new IfcTriangulatedProperties()
            {
                Coordinates = coordinates,
                TriangleIndices = triangleIndicesArr,
                Normals = normals
            };
        }

        [Pure]
        public static IfcTriangulatedProperties CreateExtrudedBodyByRefPoints(ExtrudedBodyByRefPointsTriangulatedGeometryProperties properties)
        {
            Vector<double>[] coordinates = new Vector<double>[properties.StartPoints.Length * properties.RefPoints.Length];
            for (int i = 0; i < properties.RefPoints.Length; i++)
            {
                for (int j = 0; j < properties.StartPoints.Length; j++)
                {
                    int index = i * properties.StartPoints.Length + j;
                    coordinates[index] = properties.RefPoints[i] + properties.StartPoints[j];
                }
            }

            return new IfcTriangulatedProperties()
            {
                Coordinates = coordinates,
                TriangleIndices = null,
                Normals = null
            };
        }

        [Pure]
        public static IfcTriangulatedProperties CreatePolygon(PlaneTriangulatedGeometryProperties properties)
        {
            Vector<double>[] coordinates = new Vector<double>[properties.PolygonPoints.Length];
            Array.Copy(properties.PolygonPoints, coordinates, properties.PolygonPoints.Length);
            
            int[][] triangleIndices = EarClippingTriangulator.Triangulate(properties.PolygonPoints);
            for (int i = 0; i < triangleIndices.Length; i++)
            {
                for (int j = 0; j < triangleIndices[i].Length; j++)
                {
                    triangleIndices[i][j] += 1;
                }
            }
            Vector<double>[] normals = CreateNormals(coordinates, triangleIndices);
            
            return new IfcTriangulatedProperties()
            {
                Coordinates = coordinates,
                TriangleIndices = triangleIndices,
                Normals = normals
            };
        }

        [Pure]
        private static Vector<double>[] CreateNormals(Vector<double>[] coordinates, int[][] triangleIndices)
        {
            Vector<double>[] normals = new Vector<double>[triangleIndices.Length];
            for (int i = 0; i < triangleIndices.Length; i++)
            {
                Vector<double> first = coordinates[triangleIndices[i][1] - 1] - coordinates[triangleIndices[i][0] - 1];
                Vector<double> second = coordinates[triangleIndices[i][2] - 1] - coordinates[triangleIndices[i][1] - 1];
                normals[i] = VectorExtensions.CreateNormalVector(first, second);
            }

            return normals;
        }

        [Pure]
        private static IfcTriangulatedProperties CreateCone(ConeTriangulatedGeometryProperties properties,
            Matrix<double> botMatrix, Vector<double> topConePoint)
        {
            Vector<double> botCenter = botMatrix.GetOffset();
            double radius = properties.Diameter / 2;

            Vector<double>[] coordinates = new Vector<double>[_numSegments + 1];
            Vector<double>[] normals = new Vector<double>[_numSegments * 2];
            int[][] triangleIndices = new int[_numSegments * 2][];

            coordinates[_numSegments] = topConePoint;

            for (int i = 0; i < _numSegments; i++)
            {
                double angle = 2 * Math.PI * i / _numSegments;

                double xBot = radius * Math.Cos(angle);
                double yBot = radius * Math.Sin(angle);
                Vector<double> temp = new DenseVector(new[]
                {
                    xBot, yBot, 0
                });
                coordinates[i] = botMatrix.ApplyRotation(temp) + botMatrix.GetOffset();
            }

            for (int i = 0; i < _numSegments; i++)
            {
                triangleIndices[i] = new[]
                {
                    (i + 0) % _numSegments + 1, (i + 1) % _numSegments + 1, _numSegments + 1
                };

                Vector<double> first = coordinates[triangleIndices[i][1] - 1] - coordinates[triangleIndices[i][0] - 1];
                Vector<double> second = coordinates[triangleIndices[i][2] - 1] - coordinates[triangleIndices[i][1] - 1];
                normals[i] = VectorExtensions.CreateNormalVector(first, second);
            }

            for (int i = _numSegments; i < _numSegments * 2; i++)
            {
                triangleIndices[i] = new[]
                {
                    0 + 1, (i + 1) % _numSegments + 1, (i + 2) % _numSegments + 1
                };

                Vector<double> first = coordinates[triangleIndices[i][1] - 1] - coordinates[triangleIndices[i][0] - 1];
                Vector<double> second = coordinates[triangleIndices[i][2] - 1] - coordinates[triangleIndices[i][1] - 1];
                normals[i] = VectorExtensions.CreateNormalVector(first, second);
            }

            return new IfcTriangulatedProperties
            {
                Coordinates = coordinates,
                TriangleIndices = triangleIndices,
                Normals = normals
            };
        }

        [Pure]
        private static IfcTriangulatedProperties CreateClippedCone(ClippedConeTriangulatedGeometryProperties properties,
            Matrix<double> botMatrix, Matrix<double> topMatrix)
        {
            Vector<double>[] coordinates = new Vector<double>[_numSegments * 2];
            Vector<double>[] normals = new Vector<double>[_numSegments * 4];
            int[][] triangleIndices = new int[_numSegments * 4][];

            double botRadius = properties.BottomDiameter / 2, topRadius = properties.TopDiameter / 2;
            for (int i = 0; i < _numSegments * 2; i += 2)
            {
                double angle = Math.PI * i / _numSegments;
                double cos = Math.Cos(angle), sin = Math.Sin(angle);

                double xBot = botRadius * cos;
                double yBot = botRadius * sin;
                Vector<double> bottomTemp = new DenseVector(new[]
                {
                    xBot, yBot, 0
                });
                coordinates[i] = botMatrix.ApplyRotation(bottomTemp) + botMatrix.GetOffset();

                double xTop = topRadius * cos;
                double yTop = topRadius * sin;
                Vector<double> topTemp = new DenseVector(new[]
                {
                    xTop, yTop, 0
                });
                coordinates[i + 1] = topMatrix.ApplyRotation(topTemp) + topMatrix.GetOffset();
            }

            for (int i = 0; i < _numSegments * 2; i += 2)
            {
                triangleIndices[i] = new[]
                {
                    (i + 0) % (_numSegments * 2) + 1, (i + 2) % (_numSegments * 2) + 1, (i + 3) % (_numSegments * 2) + 1
                };
                triangleIndices[i + 1] = new[]
                {
                    (i + 0) % (_numSegments * 2) + 1, (i + 3) % (_numSegments * 2) + 1, (i + 1) % (_numSegments * 2) + 1
                };

                Vector<double> first = coordinates[triangleIndices[i][1] - 1] - coordinates[triangleIndices[i][0] - 1];
                Vector<double> second = coordinates[triangleIndices[i][2] - 1] - coordinates[triangleIndices[i][1] - 1];
                normals[i] = VectorExtensions.CreateNormalVector(first, second);

                first = coordinates[triangleIndices[i + 1][1] - 1] - coordinates[triangleIndices[i + 1][0] - 1];
                second = coordinates[triangleIndices[i + 1][2] - 1] - coordinates[triangleIndices[i + 1][1] - 1];
                normals[i + 1] = VectorExtensions.CreateNormalVector(first, second);
            }

            for (int i = _numSegments * 2; i < _numSegments * 4; i += 2)
            {
                triangleIndices[i] = new[]
                {
                    0 + 1, (i + 2) % (_numSegments * 2) + 1, (i + 4) % (_numSegments * 2) + 1
                };
                triangleIndices[i + 1] = new[]
                {
                    1 + 1, (i + 3) % (_numSegments * 2) + 1, (i + 5) % (_numSegments * 2) + 1
                };

                Vector<double> first = coordinates[triangleIndices[i][1] - 1] - coordinates[triangleIndices[i][0] - 1];
                Vector<double> second = coordinates[triangleIndices[i][2] - 1] - coordinates[triangleIndices[i][1] - 1];
                normals[i] = VectorExtensions.CreateNormalVector(first, second);

                first = coordinates[triangleIndices[i + 1][1] - 1] - coordinates[triangleIndices[i + 1][0] - 1];
                second = coordinates[triangleIndices[i + 1][2] - 1] - coordinates[triangleIndices[i + 1][1] - 1];
                normals[i + 1] = VectorExtensions.CreateNormalVector(first, second);
            }

            return new IfcTriangulatedProperties
            {
                Coordinates = coordinates,
                TriangleIndices = triangleIndices,
                Normals = normals
            };
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
    
    public static class EarClippingTriangulator
    {
        public static int[][] Triangulate(IReadOnlyList<Vector<double>> vertices)
        {
            Vector<double>[] verticesArr = vertices as Vector<double>[] ?? vertices.ToArray();
            PlaneProjection projection = PlaneProjectionFactory.Create(verticesArr);
            Vector2[] points2D = projection.Project(verticesArr);
            
            if (points2D.Length < 3)
                throw new ArgumentException("Polygon must contain at least 3 vertices");

            List<int[]> triangles = new List<int[]>();
            List<int> polygon = new List<int>(points2D.Length);

            for (int i = 0; i < vertices.Count; i++)
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
                    
                    triangles.Add(new int[] { prev, curr, next });
                    polygon.RemoveAt(i);

                    earFound = true;
                    break;
                }

                if (!earFound)
                    throw new InvalidOperationException("Failed to triangulate polygon. Polygon may be self-intersecting");
            }
            
            triangles.Add(new int[] { polygon[0], polygon[1], polygon[2] });
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
}