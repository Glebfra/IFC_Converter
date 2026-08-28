using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Geometry.MeshBuilders
{
    public sealed class TorusSegmentMeshBuilder : MeshBuilder
    {
        private readonly Vector<double> _center;
        private readonly Vector<double> _start;
        private readonly Vector<double> _end;

        private readonly double _radius;

        private readonly int _arcSegments;
        private readonly int _profileSegments;

        public TorusSegmentMeshBuilder(Vector<double> center, Vector<double> start, Vector<double> end, double radius, int arcSegments = 32, int profileSegments = 16)
        {
            _center = center;
            _start = start;
            _end = end;
            _radius = radius;
            _arcSegments = arcSegments;
            _profileSegments = profileSegments;
        }

        public override IMesh Build()
        {
            Vector<double> startVector = _start - _center;
            Vector<double> endVector = _end - _center;

            double torusRadius = startVector.L2Norm();

            Vector<double> u = startVector.Normalize(2);
            Vector<double> n = startVector.CrossProduct(endVector).Normalize(2);
            Vector<double> v = n.CrossProduct(u).Normalize(2);

            double angle = Math.Atan2(endVector.DotProduct(v), endVector.DotProduct(u));

            List<Vector<double>> vertices = new List<Vector<double>>((_arcSegments + 1) * _profileSegments);
            List<int[]> triangles = new List<int[]>(_arcSegments * _profileSegments * 2);
            
            for (int i = 0; i <= _arcSegments; i++)
            {
                double theta =
                    angle * i / _arcSegments;

                double cosTheta = Math.Cos(theta);
                double sinTheta = Math.Sin(theta);
                
                Vector<double> radial = cosTheta * u + sinTheta * v;
                Vector<double> sectionCenter = _center + torusRadius * radial;

                for (int j = 0; j < _profileSegments; j++)
                {
                    double phi = 2.0 * Math.PI * j / _profileSegments;

                    double cosPhi = Math.Cos(phi);
                    double sinPhi = Math.Sin(phi);

                    Vector<double> point = sectionCenter + _radius * (cosPhi * radial + sinPhi * n);
                    vertices.Add(point);
                }
            }
            
            for (int i = 0; i < _arcSegments; i++)
            {
                int currentRow = i * _profileSegments;
                int nextRow = (i + 1) * _profileSegments;

                for (int j = 0; j < _profileSegments; j++)
                {
                    int nextProfile = (j + 1) % _profileSegments;

                    int a = currentRow + j;
                    int b = currentRow + nextProfile;
                    int c = nextRow + nextProfile;
                    int d = nextRow + j;

                    triangles.Add(new[] { a, b, c });
                    triangles.Add(new[] { a, c, d });
                }
            }

            return BuildMesh(vertices.ToArray(), triangles.ToArray());
        }
    }
}