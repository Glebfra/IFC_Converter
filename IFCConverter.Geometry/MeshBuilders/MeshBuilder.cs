using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.MeshBuilders
{
    public abstract class MeshBuilder : IMeshBuilder
    {
        [Pure]
        public abstract IMesh Build();

        protected static void AddRectangle(List<int[]> triangles, int a, int b, int c, int d)
        {
            AddTriangle(triangles, a, b, c);
            AddTriangle(triangles, b, d, c);
        }

        protected static void AddTriangle(List<int[]> triangles, int a, int b, int c)
        {
            triangles.Add(new[]
            {
                a, b, c
            });
        }

        [Pure]
        protected static FixedVector<Dim3>[] CreateNormals(FixedVector<Dim3>[] vertices, int[][] triangles)
        {
            FixedVector<Dim3>[] normals = new FixedVector<Dim3>[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                int[] triangle = triangles[i];

                FixedVector<Dim3> first = vertices[triangle[1]] - vertices[triangle[0]];
                FixedVector<Dim3> second = vertices[triangle[2]] - vertices[triangle[1]];
                normals[i] = first.CreateNormalVector(second);
            }

            return normals;
        }

        [Pure]
        protected static Mesh BuildMesh(FixedVector<Dim3>[] vertices, int[][] triangles)
        {
            FixedVector<Dim3>[] normals = CreateNormals(vertices, triangles);
            return new Mesh(vertices, triangles, normals);
        }
    }
}