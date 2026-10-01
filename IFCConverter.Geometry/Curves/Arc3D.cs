using System.Diagnostics.Contracts;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Curves
{
    public sealed class Arc3D : ICurve3D
    {
        public Circle3D Circle { get; }
        
        public double StartAngle { get; }
        public double EndAngle { get; }
        
        public Arc3D(Circle3D circle, double startAngle, double endAngle)
        {
            Circle = circle;
            StartAngle = startAngle;
            EndAngle = endAngle;
        }

        [Pure]
        public double GetAngle(double parameter)
        {
            return StartAngle + (EndAngle - StartAngle) * parameter;
        }
        
        [Pure]
        public FixedVector<Dim3> GetPoint(double parameter)
        {
            return Circle.GetPoint(GetAngle(parameter));
        }

        [Pure]
        public FixedVector<Dim3> GetTangent(double parameter)
        {
            return Circle.GetTangent(GetAngle(parameter));
        }
    }
}