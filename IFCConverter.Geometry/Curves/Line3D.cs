using System.Diagnostics.Contracts;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Curves
{
    public sealed class Line3D : ICurve3D
    {
        public FixedVector<Dim3> Origin { get; }
        public FixedVector<Dim3> Direction { get; }
        
        public Line3D(FixedVector<Dim3> origin, FixedVector<Dim3> direction)
        {
            Origin = origin;
            Direction = direction.Normalize();
        }
        
        [Pure]
        public FixedVector<Dim3> GetPoint(double parameter)
        {
            return Origin + Direction * parameter;
        }

        [Pure]
        public FixedVector<Dim3> GetTangent(double parameter)
        {
            return Direction;
        }
    }
}