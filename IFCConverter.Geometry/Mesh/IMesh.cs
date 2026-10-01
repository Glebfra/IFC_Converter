using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh
{
    public interface IMesh : IGeometry
    {
        IReadOnlyList<FixedVector<Dim3>> Vertices { get; }
        IReadOnlyList<Triangle> Triangles { get; }
        IReadOnlyList<FixedVector<Dim3>> Normals { get; }
        
        int VertexCount { get; }
        int TriangleCount { get; }
        int NormalCount { get; }

        int AddVertex(FixedVector<Dim3> vertex);
        int AddTriangle(Triangle triangle);
        int AddNormal(FixedVector<Dim3> normal);
    }
}