using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Geometry
{
    public interface IMesh
    {
        Vector<double>[] Vertices { get; }
        int[][] Triangles { get; }
        Vector<double>[] Normals { get; }
    }
}