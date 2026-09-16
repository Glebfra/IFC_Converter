using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Utils.Geometry;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.IFC.Geometries
{
    public struct ConeTriangulatedGeometryProperties
    {
        public FixedVector<Dim3> TopConePoint;
        public FixedVector<Dim3> BottomConeCenter;
        public double Diameter;
    }

    public struct ClippedConeTriangulatedGeometryProperties
    {
        public FixedVector<Dim3> TopConeCenter;
        public FixedVector<Dim3> BottomConeCenter;
        public FixedVector<Dim3> Direction;
        public double TopDiameter;
        public double BottomDiameter;
    }

    public struct SphereTriangulatedGeometryProperties
    {
        public FixedVector<Dim3> Center;
        public double Diameter;
    }

    public struct ExtrudedBodyTriangulatedGeometryProperties
    {
        public FixedVector<Dim3>[] StartPoints;
        public FixedVector<Dim3> ExtrudedDirection;
        public double Length;
    }

    public struct ExtrudedBodyByRefPointsTriangulatedGeometryProperties
    {
        public FixedVector<Dim3>[] StartPoints;
        public FixedVector<Dim3>[] RefPoints;
    }

    public struct PlaneTriangulatedGeometryProperties
    {
        public FixedVector<Dim3>[] PolygonPoints;
    }

    public struct IfcTriangulatedProperties
    {
        public IEnumerable<FixedVector<Dim3>> Coordinates;
        public IEnumerable<IEnumerable<int>> TriangleIndices;
        public IEnumerable<FixedVector<Dim3>> Normals;

        private const int _numSegments = 16;

        [Pure]
        public static IfcTriangulatedProperties CreateSphere(SphereTriangulatedGeometryProperties properties)
        {
            double radius = properties.Diameter / 2;

            FixedVector<Dim3>[] coordinates = new FixedVector<Dim3>[_numSegments * _numSegments];
            FixedVector<Dim3>[] normals = new FixedVector<Dim3>[_numSegments * (_numSegments - 1) * 2];
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
                    FixedVector<Dim3> temp = FixedVector<Dim3>.Builder.Dense(x, y, z);

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

                FixedVector<Dim3> first = coordinates[triangleIndices[arrIndex][1] - 1]
                                          - coordinates[triangleIndices[arrIndex][0] - 1];
                FixedVector<Dim3> second = coordinates[triangleIndices[arrIndex][2] - 1]
                                           - coordinates[triangleIndices[arrIndex][1] - 1];
                normals[arrIndex] = first.CreateNormalVector(second);

                first = coordinates[triangleIndices[arrIndex + 1][1] - 1]
                        - coordinates[triangleIndices[arrIndex + 1][0] - 1];
                second = coordinates[triangleIndices[arrIndex + 1][2] - 1]
                         - coordinates[triangleIndices[arrIndex + 1][1] - 1];
                normals[arrIndex + 1] = first.CreateNormalVector(second);

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
            FixedVector<Dim3> zAxis = properties.TopConePoint - properties.BottomConeCenter;
            FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
            FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

            FixedMatrix<Dim4> botMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(properties.BottomConeCenter, xAxis, yAxis, zAxis);

            return CreateCone(properties, botMatrix, properties.TopConePoint);
        }

        [Pure]
        public static IfcTriangulatedProperties CreateClippedCone(ClippedConeTriangulatedGeometryProperties properties)
        {
            FixedVector<Dim3> heightDirection = properties.Direction
                                                ?? properties.TopConeCenter - properties.BottomConeCenter;
            FixedVector<Dim3> zAxis = heightDirection.Normalize();
            FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
            FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

            FixedMatrix<Dim4> botMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(properties.BottomConeCenter, xAxis, yAxis, zAxis);
            FixedMatrix<Dim4> topMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(properties.TopConeCenter, xAxis, yAxis, zAxis);

            return CreateClippedCone(properties, botMatrix, topMatrix);
        }

        [Pure]
        public static IfcTriangulatedProperties CreateExtrudedBody(ExtrudedBodyTriangulatedGeometryProperties properties)
        {
            FixedVector<Dim3>[] coordinates = new FixedVector<Dim3>[properties.StartPoints.Length * 2];

            List<int[]> triangleIndices = new List<int[]>(properties.StartPoints.Length * 2);

            for (int i = 0; i < properties.StartPoints.Length; i++)
            {
                coordinates[i] = properties.StartPoints[i];
                coordinates[i + properties.StartPoints.Length] = properties.StartPoints[i] + properties.ExtrudedDirection * properties.Length;
            }

            FixedVector<Dim3>[] startPolygon = coordinates.Take(properties.StartPoints.Length).ToArray();
            FixedVector<Dim3>[] endPolygon = coordinates.Skip(properties.StartPoints.Length).ToArray();

            for (int i = 0; i < properties.StartPoints.Length; i++)
            {
                int a = i;
                int b = i + properties.StartPoints.Length;
                int c = (i + 1) % properties.StartPoints.Length;
                int d = (i + 1) % properties.StartPoints.Length + properties.StartPoints.Length;
                triangleIndices.AddRange(CreateRectanglePolygon(a, b, c, d));
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

            FixedVector<Dim3>[] normals = CreateNormals(coordinates, triangleIndicesArr);

            return new IfcTriangulatedProperties
            {
                Coordinates = coordinates,
                TriangleIndices = triangleIndicesArr,
                Normals = normals
            };
        }

        [Pure]
        public static IfcTriangulatedProperties CreateExtrudedBodyByRefPoints(ExtrudedBodyByRefPointsTriangulatedGeometryProperties properties)
        {
            int profileSize = properties.StartPoints.Length;
            int sectionsCount = properties.RefPoints.Length;

            if (sectionsCount < 2)
                throw new ArgumentException("RefPoints must contain at least two points.");

            FixedVector<Dim3>[] coordinates = new FixedVector<Dim3>[profileSize * sectionsCount];
            List<int[]> triangleIndices = new List<int[]>(profileSize * sectionsCount * 2);

            for (int i = 0; i < sectionsCount; i++)
            {
                for (int j = 0; j < profileSize; j++)
                {
                    int index = i * profileSize + j;
                    coordinates[index] = properties.StartPoints[j] + properties.RefPoints[i];
                }
            }

            for (int i = 0; i < sectionsCount - 1; i++)
            {
                for (int j = 0; j < profileSize; j++)
                {
                    int next = (j + 1) % profileSize;

                    int a = i * profileSize + j;
                    int b = i * profileSize + next;
                    int c = (i + 1) * profileSize + j;
                    int d = (i + 1) * profileSize + next;
                    triangleIndices.AddRange(CreateRectanglePolygon(a, b, c, d));
                }
            }

            int[][] startCap = EarClippingTriangulator.Triangulate(coordinates.Take(profileSize).ToArray());
            int[][] endCap = EarClippingTriangulator.Triangulate(coordinates.Skip(profileSize * (sectionsCount - 1)).Take(profileSize).ToArray());

            for (int i = 0; i < startCap.Length; i++)
            {
                for (int j = 0; j < startCap[i].Length; j++)
                {
                    startCap[i][j] += 1;
                }
            }

            for (int i = 0; i < endCap.Length; i++)
            {
                for (int j = 0; j < endCap[i].Length; j++)
                {
                    endCap[i][j] += 1;
                }
            }

            triangleIndices.AddRange(startCap);
            triangleIndices.AddRange(endCap);

            int[][] triangleIndicesArr = triangleIndices.ToArray();
            FixedVector<Dim3>[] normals = CreateNormals(coordinates, triangleIndicesArr);

            return new IfcTriangulatedProperties
            {
                Coordinates = coordinates,
                TriangleIndices = triangleIndicesArr,
                Normals = normals
            };
        }

        [Pure]
        public static IfcTriangulatedProperties CreatePolygon(PlaneTriangulatedGeometryProperties properties)
        {
            FixedVector<Dim3>[] coordinates = new FixedVector<Dim3>[properties.PolygonPoints.Length];
            Array.Copy(properties.PolygonPoints, coordinates, properties.PolygonPoints.Length);

            int[][] triangleIndices = EarClippingTriangulator.Triangulate(properties.PolygonPoints);
            for (int i = 0; i < triangleIndices.Length; i++)
            {
                for (int j = 0; j < triangleIndices[i].Length; j++)
                {
                    triangleIndices[i][j] += 1;
                }
            }

            FixedVector<Dim3>[] normals = CreateNormals(coordinates, triangleIndices);

            return new IfcTriangulatedProperties
            {
                Coordinates = coordinates,
                TriangleIndices = triangleIndices,
                Normals = normals
            };
        }

        [Pure]
        private static int[][] CreateRectanglePolygon(int a, int b, int c, int d)
        {
            return new[]
            {
                new[]
                {
                    a + 1, b + 1, c + 1
                },
                new[]
                {
                    b + 1, d + 1, c + 1
                }
            };
        }

        [Pure]
        private static int[][] CreateTrianglePolygon(int a, int b, int c)
        {
            return new[]
            {
                new[]
                {
                    a + 1, b + 1, c + 1
                }
            };
        }

        [Pure]
        private static FixedVector<Dim3>[] CreateNormals(FixedVector<Dim3>[] coordinates, int[][] triangleIndices)
        {
            FixedVector<Dim3>[] normals = new FixedVector<Dim3>[triangleIndices.Length];
            for (int i = 0; i < triangleIndices.Length; i++)
            {
                FixedVector<Dim3> first = coordinates[triangleIndices[i][1] - 1] - coordinates[triangleIndices[i][0] - 1];
                FixedVector<Dim3> second = coordinates[triangleIndices[i][2] - 1] - coordinates[triangleIndices[i][1] - 1];
                normals[i] = first.CreateNormalVector(second);
            }

            return normals;
        }

        [Pure]
        private static IfcTriangulatedProperties CreateCone(ConeTriangulatedGeometryProperties properties,
            FixedMatrix<Dim4> botMatrix, FixedVector<Dim3> topConePoint)
        {
            FixedVector<Dim3> botCenter = botMatrix.GetOffset().ToCartesian();
            double radius = properties.Diameter / 2;

            FixedVector<Dim3>[] coordinates = new FixedVector<Dim3>[_numSegments + 1];
            FixedVector<Dim3>[] normals = new FixedVector<Dim3>[_numSegments * 2];
            int[][] triangleIndices = new int[_numSegments * 2][];

            coordinates[_numSegments] = topConePoint;

            for (int i = 0; i < _numSegments; i++)
            {
                double angle = 2 * Math.PI * i / _numSegments;

                double xBot = radius * Math.Cos(angle);
                double yBot = radius * Math.Sin(angle);
                FixedVector<Dim3> temp = FixedVector<Dim3>.Builder.Dense(xBot, yBot, 0);
                coordinates[i] = botMatrix.GetRotation() * temp + botMatrix.GetTranslation();
            }

            for (int i = 0; i < _numSegments; i++)
            {
                triangleIndices[i] = new[]
                {
                    (i + 0) % _numSegments + 1, (i + 1) % _numSegments + 1, _numSegments + 1
                };

                FixedVector<Dim3> first = coordinates[triangleIndices[i][1] - 1] - coordinates[triangleIndices[i][0] - 1];
                FixedVector<Dim3> second = coordinates[triangleIndices[i][2] - 1] - coordinates[triangleIndices[i][1] - 1];
                normals[i] = first.CreateNormalVector(second);
            }

            for (int i = _numSegments; i < _numSegments * 2; i++)
            {
                triangleIndices[i] = new[]
                {
                    0 + 1, (i + 1) % _numSegments + 1, (i + 2) % _numSegments + 1
                };

                FixedVector<Dim3> first = coordinates[triangleIndices[i][1] - 1] - coordinates[triangleIndices[i][0] - 1];
                FixedVector<Dim3> second = coordinates[triangleIndices[i][2] - 1] - coordinates[triangleIndices[i][1] - 1];
                normals[i] = first.CreateNormalVector(second);
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
            FixedMatrix<Dim4> botMatrix, FixedMatrix<Dim4> topMatrix)
        {
            FixedVector<Dim3>[] coordinates = new FixedVector<Dim3>[_numSegments * 2];
            FixedVector<Dim3>[] normals = new FixedVector<Dim3>[_numSegments * 4];
            int[][] triangleIndices = new int[_numSegments * 4][];

            double botRadius = properties.BottomDiameter / 2, topRadius = properties.TopDiameter / 2;
            for (int i = 0; i < _numSegments * 2; i += 2)
            {
                double angle = Math.PI * i / _numSegments;
                double cos = Math.Cos(angle), sin = Math.Sin(angle);

                double xBot = botRadius * cos;
                double yBot = botRadius * sin;
                FixedVector<Dim3> bottomTemp = FixedVector<Dim3>.Builder.Dense(xBot, yBot, 0);
                coordinates[i] = botMatrix.GetRotation() * bottomTemp + botMatrix.GetTranslation();

                double xTop = topRadius * cos;
                double yTop = topRadius * sin;
                FixedVector<Dim3> topTemp = FixedVector<Dim3>.Builder.Dense(xTop, yTop, 0);
                coordinates[i + 1] = topMatrix.GetRotation() * topTemp + topMatrix.GetTranslation();
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

                FixedVector<Dim3> first = coordinates[triangleIndices[i][1] - 1] - coordinates[triangleIndices[i][0] - 1];
                FixedVector<Dim3> second = coordinates[triangleIndices[i][2] - 1] - coordinates[triangleIndices[i][1] - 1];
                normals[i] = first.CreateNormalVector(second);

                first = coordinates[triangleIndices[i + 1][1] - 1] - coordinates[triangleIndices[i + 1][0] - 1];
                second = coordinates[triangleIndices[i + 1][2] - 1] - coordinates[triangleIndices[i + 1][1] - 1];
                normals[i + 1] = first.CreateNormalVector(second);
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

                FixedVector<Dim3> first = coordinates[triangleIndices[i][1] - 1] - coordinates[triangleIndices[i][0] - 1];
                FixedVector<Dim3> second = coordinates[triangleIndices[i][2] - 1] - coordinates[triangleIndices[i][1] - 1];
                normals[i] = first.CreateNormalVector(second);

                first = coordinates[triangleIndices[i + 1][1] - 1] - coordinates[triangleIndices[i + 1][0] - 1];
                second = coordinates[triangleIndices[i + 1][2] - 1] - coordinates[triangleIndices[i + 1][1] - 1];
                normals[i + 1] = first.CreateNormalVector(second);
            }

            return new IfcTriangulatedProperties
            {
                Coordinates = coordinates,
                TriangleIndices = triangleIndices,
                Normals = normals
            };
        }
    }
}