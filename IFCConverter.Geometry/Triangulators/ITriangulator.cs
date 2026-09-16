using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Triangulators
{
    internal interface ITriangulator
    {
        int[][] Triangulate(FixedVector<Dim3>[] vertices);
    }
}