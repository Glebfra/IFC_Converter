using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Geometry;
using IFCConverter.Geometry.Algorithms;
using IFCConverter.Geometry.Boundary;
using IFCConverter.Geometry.Mesh;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Importer.Extensions
{
    public static class MeshExtensions
    {
        [Pure]
        public static FixedVector<Dim3>[][] GetBoundaryVertices(this IMesh mesh)
        {
            Edge[] boundaryEdges = ClosedMeshTopology.GetBoundaryEdges(mesh).ToArray();
            List<BoundaryLoop> boundaryLoops = BoundaryLoopBuilder.Build(boundaryEdges);
            return boundaryLoops
                .Select(boundaryLoop => boundaryLoop.Vertices.Select(index => mesh.Vertices[index]).ToArray())
                .ToArray();
        }
    }
}