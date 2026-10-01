using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry
{
    public interface IMesh : IGeometry
    {
        IReadOnlyList<FixedVector<Dim3>> Vertices { get; }
        IReadOnlyList<Triangle> Triangles { get; }
        IReadOnlyList<FixedVector<Dim3>> FaceNormals { get; }
        
        int VertexCount { get; }
        int TriangleCount { get; }
        int FaceNormalCount { get; }

        int AddVertex(FixedVector<Dim3> vertex);
        int AddTriangle(Triangle triangle);
        int AddFaceNormal(FixedVector<Dim3> faceNormal);
    }
}