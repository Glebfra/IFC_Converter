using System;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Geometry
{
    public sealed class Mesh : IMesh
    {
        public Vector<double>[] Vertices { get; }
        public int[][] Triangles { get; }
        public Vector<double>[] Normals { get; }

        public Mesh(Vector<double>[] vertices, int[][] triangles, Vector<double>[] normals)
        {
            Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            Triangles = triangles ?? throw new ArgumentNullException(nameof(triangles));
            Normals = normals ?? throw new ArgumentNullException(nameof(normals));
        }
    }
}