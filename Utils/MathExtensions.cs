using System;
using System.Diagnostics.Contracts;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Utils
{
    public static class MathExtensions
    {
        [Pure]
        public static double CalculateTorusSegmentLength(double radius, double angle)
        {
            return radius * Math.Tan(angle / 2);
        }

        [Pure]
        public static double CalculateAnchorDisplacement(Matrix<double> segmentMatrix, double diameter)
        {
            double angle = segmentMatrix.GetZ().Angle(VectorExtensions.Z);
            if (angle.AlmostEqual(0, 1e-6)) // a!=0 => sin(a)!=0
                angle = segmentMatrix.GetY().Angle(VectorExtensions.Z);

            return diameter / (2 * Math.Sin(angle)); // r / sin(a)
        }
    }
}