using System;
using System.Diagnostics.Contracts;
using IFCConverter.Geometry.Triangulators;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.MeshBuilders
{
    public sealed class EarClippingMeshBuilder : MeshBuilder
    {
        private readonly ITriangulator _triangulator = new EarClippingTriangulator();
        private readonly FixedVector<Dim3>[] _vertices;

        public EarClippingMeshBuilder(FixedVector<Dim3>[] vertices)
        {
            _vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
        }

        [Pure]
        public override IMesh Build()
        {
            int[][] triangles = _triangulator.Triangulate(_vertices);
            return BuildMesh(_vertices, triangles);
        }
    }
}