using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Geometry.MeshBuilders
{
    public abstract class MeshBuilder : IMeshBuilder
    {
        [Pure]
        public abstract IMesh Build();

        [Pure]
        protected static Vector<double> CreateVector(double x, double y, double z)
        {
            return new DenseVector(new[]
            {
                x, y, z
            });
        }

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
        protected static Vector<double>[] CreateNormals(Vector<double>[] vertices, int[][] triangles)
        {
            Vector<double>[] normals = new Vector<double>[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                int[] triangle = triangles[i];

                Vector<double> first = vertices[triangle[1]] - vertices[triangle[0]];
                Vector<double> second = vertices[triangle[2]] - vertices[triangle[1]];
                normals[i] = VectorExtensions.CreateNormalVector(first, second);
            }

            return normals;
        }

        [Pure]
        protected static Mesh BuildMesh(Vector<double>[] vertices, int[][] triangles)
        {
            Vector<double>[] normals = CreateNormals(vertices, triangles);
            return new Mesh(vertices, triangles, normals);
        }
    }
}