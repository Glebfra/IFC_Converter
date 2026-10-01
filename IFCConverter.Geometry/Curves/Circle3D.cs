using System;
using System.Diagnostics.Contracts;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Curves
{
    public readonly struct Circle3D : ICurve3D
    {
        public FixedVector<Dim3> Center { get; }
        
        public FixedVector<Dim3> BasisX { get; }
        public FixedVector<Dim3> BasisY { get; }
        
        public double Radius { get; }
        
        public Circle3D(FixedVector<Dim3> center, FixedVector<Dim3> basisX, FixedVector<Dim3> basisY, double radius)
        {
            Center = center;
            BasisX = basisX.Normalize();
            BasisY = basisY.Normalize();
            Radius = radius;
        }
        
        [Pure]
        public FixedVector<Dim3> GetPoint(double parameter)
        {
            return Center + BasisX * (Radius * Math.Cos(parameter)) + BasisY * (Radius * Math.Sin(parameter));
        }

        [Pure]
        public FixedVector<Dim3> GetTangent(double parameter)
        {
            return (BasisX * -Math.Sin(parameter) + BasisY * Math.Cos(parameter)).Normalize();
        }
    }
}