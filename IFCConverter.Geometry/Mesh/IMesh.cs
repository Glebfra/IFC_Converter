using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh
{
    public interface IMesh
    {
        FixedVector<Dim3>[] Vertices { get; }
        int[][] Triangles { get; }
        FixedVector<Dim3>[] Normals { get; }

        IEnumerable<Triangle> GetTriangles();
    }
}