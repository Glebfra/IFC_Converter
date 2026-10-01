using System;
using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Algorithms
{
    public static class MeshGeometry
    {
        public static FixedVector<Dim3> GetFaceNormal(IMesh mesh, int triangleIndex)
        {
            Triangle triangle = mesh.Triangles[triangleIndex];
            
            FixedVector<Dim3> a = mesh.Vertices[triangle.A];
            FixedVector<Dim3> b = mesh.Vertices[triangle.B];
            FixedVector<Dim3> c = mesh.Vertices[triangle.C];

            return GetFaceNormal(a, b, c);
        }

        public static FixedVector<Dim3> GetFaceNormal(IReadOnlyList<FixedVector<Dim3>> triangleVertices)
        {
            if (triangleVertices.Count != 3)
                throw new ArgumentException("Triangle vertices should contain exact 3 vertices", nameof(triangleVertices));

            FixedVector<Dim3> a = triangleVertices[0];
            FixedVector<Dim3> b = triangleVertices[1];
            FixedVector<Dim3> c = triangleVertices[2];
            return GetFaceNormal(a, b, c);
        }

        public static FixedVector<Dim3> GetFaceNormal(FixedVector<Dim3> a, FixedVector<Dim3> b, FixedVector<Dim3> c)
        {
            return (b - a).CrossProduct(c - a).Normalize();
        }
    }
}