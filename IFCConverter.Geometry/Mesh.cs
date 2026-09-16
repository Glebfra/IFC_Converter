using System;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry
{
    public sealed class Mesh : IMesh
    {
        public Mesh(FixedVector<Dim3>[] vertices, int[][] triangles, FixedVector<Dim3>[] normals)
        {
            Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            Triangles = triangles ?? throw new ArgumentNullException(nameof(triangles));
            Normals = normals ?? throw new ArgumentNullException(nameof(normals));
        }

        public FixedVector<Dim3>[] Vertices { get; }
        public int[][] Triangles { get; }
        public FixedVector<Dim3>[] Normals { get; }
    }
}