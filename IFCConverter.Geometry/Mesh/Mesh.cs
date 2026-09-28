using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh
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

        public IEnumerable<Triangle> GetTriangles()
        {
            return Triangles.Select(triangle => new Triangle(triangle[0], triangle[1], triangle[2]));
        }
    }
}