using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Curves
{
    public interface ICurve3D : IGeometry
    {
        FixedVector<Dim3> GetPoint(double parameter);
        FixedVector<Dim3> GetTangent(double parameter);
    }
}