using IFCConverter.Geometry.Mesh;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Triangulators
{
    internal interface ITriangulator
    {
        Triangle[] Triangulate(FixedVector<Dim3>[] vertices);
    }
}