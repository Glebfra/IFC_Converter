using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Geometry.Triangulators
{
    internal interface ITriangulator
    {
        int[][] Triangulate(Vector<double>[] vertices);
    }
}