using System;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Geometry.MeshBuilders
{
    public sealed class EarClippingMeshBuilder : MeshBuilder
    {
        private readonly Vector<double>[] _vertices;
        
        public EarClippingMeshBuilder(Vector<double>[] vertices)
        {
            _vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
        }

        [Pure]
        public override IMesh Build()
        {
            int[][] triangles = EarClippingTriangulator.Triangulate(_vertices);
            return BuildMesh(_vertices, triangles);
        }
    }
}