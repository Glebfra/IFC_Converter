using System;
using IFCConverter.Geometry.Triangulators;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh.Builders
{
    public sealed class EarClippingAlgorithmBuilder : MeshBuilder
    {
        public EarClippingAlgorithmBuilder(params FixedVector<Dim3>[] vertices)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));

            if (vertices.Length < 3)
                throw new ArgumentException("Polygon must contain at least 3 vertices.", nameof(vertices));

            Triangle[] triangles = new EarClippingTriangulator().Triangulate(vertices);

            int[] vertexIndices = new int[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertexIndices[i] = AddVertex(vertices[i]);
            }
            
            foreach (Triangle triangle in triangles)
            {
                AddTriangle(new Triangle(
                    vertexIndices[triangle.A],
                    vertexIndices[triangle.B],
                    vertexIndices[triangle.C]
                ));
            }
        }
    }
}