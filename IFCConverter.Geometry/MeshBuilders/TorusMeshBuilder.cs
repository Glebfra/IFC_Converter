using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Geometry.MeshBuilders
{
    public sealed class TorusMeshBuilder : MeshBuilder
    {
        private readonly double _majorRadius;
        private readonly double _minorRadius;
        private readonly Matrix<double> _transform;

        private readonly int _sections;
        private readonly int _profileSize;

        public TorusMeshBuilder(double majorRadius, double minorRadius, Matrix<double> transform, int sections = 16, int profileSize = 16)
        {
            if (majorRadius <= 0)
                throw new ArgumentOutOfRangeException(nameof(majorRadius));
            if (minorRadius <= 0)
                throw new ArgumentOutOfRangeException(nameof(minorRadius));
            if (sections < 2)
                throw new ArgumentOutOfRangeException(nameof(sections));
            if (profileSize < 3)
                throw new ArgumentOutOfRangeException(nameof(profileSize));
            
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            
            _majorRadius = majorRadius;
            _minorRadius = minorRadius;
            _sections = sections;
            _profileSize = profileSize;
        }

        [Pure]
        public override IMesh Build()
        {
            return BuildSegment(2 * Math.PI, true);
        }

        [Pure]
        public IMesh BuildSegment(double angle)
        {
            return BuildSegment(angle, false);
        }

        [Pure]
        private IMesh BuildSegment(double angle, bool closed)
        {
            Vector<double>[] vertices = CreateVertices(angle);
            int[][] triangles = CreateTriangles(closed);

            return BuildMesh(vertices, triangles);
        }

        [Pure]
        private Vector<double>[] CreateVertices(double angle)
        {
            Vector<double>[] vertices = new Vector<double>[_sections * _profileSize];

            Vector<double> position = _transform.GetOffset();
            Matrix<double> rotation = _transform.GetRotation();

            for (int i = 0; i < _sections; i++)
            {
                double psi = angle * i / (_sections - 1);

                double psiCos = Math.Cos(psi);
                double psiSin = Math.Sin(psi);
                
                for (int j = 0; j < _profileSize; j++)
                {
                    double phi = 2 * Math.PI * j / (_profileSize - 1);

                    Vector<double> vertex = CreateVector(
                        (_majorRadius + _minorRadius * psiCos) * Math.Cos(phi),
                        (_majorRadius + _minorRadius * psiCos) * Math.Sin(phi),
                        _minorRadius * psiSin);

                    vertices[i * _profileSize + j] = rotation.Multiply(vertex) + position;
                }
            }

            return vertices;
        }

        [Pure]
        private int[][] CreateTriangles(bool closed)
        {
            List<int[]> triangles = new List<int[]>();
            int sectionCount = closed ? _sections : _sections - 1;

            for (int i = 0; i < sectionCount; i++)
            {
                int nextSection = (i + 1) % _sections;

                for (int j = 0; j < _profileSize - 1; j++)
                {
                    int a = i * _profileSize + j;
                    int b = i * _profileSize + j + 1;
                    int c = nextSection * _profileSize + j;
                    int d = nextSection * _profileSize + j + 1;
                    
                    AddRectangle(triangles, a, b, c, d);
                }
            }

            return triangles.ToArray();
        }
    }
}