using System;
using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.MeshBuilders
{
    public sealed class TorusSegmentMeshBuilder : MeshBuilder
    {

        private readonly int _arcSegments;
        private readonly FixedVector<Dim3> _center;
        private readonly FixedVector<Dim3> _end;
        private readonly int _profileSegments;

        private readonly double _radius;
        private readonly FixedVector<Dim3> _start;

        public TorusSegmentMeshBuilder(FixedVector<Dim3> center, FixedVector<Dim3> start, FixedVector<Dim3> end, double radius, int arcSegments = 32,
            int profileSegments = 16)
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
            FixedVector<Dim3> startVector = _start - _center;
            FixedVector<Dim3> endVector = _end - _center;

            double torusRadius = startVector.L2Norm();

            FixedVector<Dim3> u = startVector.Normalize();
            FixedVector<Dim3> n = startVector.CrossProduct(endVector).Normalize();
            FixedVector<Dim3> v = n.CrossProduct(u).Normalize();

            double angle = Math.Atan2(endVector.Dot(v), endVector.Dot(u));

            List<FixedVector<Dim3>> vertices = new List<FixedVector<Dim3>>((_arcSegments + 1) * _profileSegments);
            List<int[]> triangles = new List<int[]>(_arcSegments * _profileSegments * 2);

            for (int i = 0; i <= _arcSegments; i++)
            {
                double theta =
                    angle * i / _arcSegments;

                double cosTheta = Math.Cos(theta);
                double sinTheta = Math.Sin(theta);

                FixedVector<Dim3> radial = cosTheta * u + sinTheta * v;
                FixedVector<Dim3> sectionCenter = _center + torusRadius * radial;

                for (int j = 0; j < _profileSegments; j++)
                {
                    double phi = 2.0 * Math.PI * j / _profileSegments;

                    double cosPhi = Math.Cos(phi);
                    double sinPhi = Math.Sin(phi);

                    FixedVector<Dim3> point = sectionCenter + _radius * (cosPhi * radial + sinPhi * n);
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

                    triangles.Add(new[]
                    {
                        a, b, c
                    });
                    triangles.Add(new[]
                    {
                        a, c, d
                    });
                }
            }

            return BuildMesh(vertices.ToArray(), triangles.ToArray());
        }
    }
}