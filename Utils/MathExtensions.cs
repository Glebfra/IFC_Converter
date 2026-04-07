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
        
        public const double g = 9.80665;

        public const double TToKg = 1000.0;
        public const double KgToT = 1 / TToKg;

        public const double TfToN = TToKg * g;
        public const double NToTf = 1 / TfToN;
        
        public const double TfToKg = 1000.0;
        public const double KgToTf = 1 / TfToKg;
        
        public const double T_m2ToPa = TToKg * g;
        public const double PaToT_m2 = 1 / T_m2ToPa;

        public const double MToMm = 1000.0;
        public const double MmToM = 1 / MToMm;
    }
}