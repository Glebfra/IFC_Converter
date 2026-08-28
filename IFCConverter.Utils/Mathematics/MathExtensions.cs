using System;
using System.Diagnostics.Contracts;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace IFCConverter.Utils.Mathematics
{
    public static class MathExtensions
    {
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

        [Pure]
        public static Vector<double>[][] CreateTorus(Matrix<double> transformation, double profileRadius, double sectionsRadius,
            int profileSize, int sectionsCount)
        {
            return CreateTorusSegment(transformation, profileRadius, sectionsRadius, 2 * Math.PI, profileSize, sectionsCount);
        }

        [Pure]
        public static Vector<double>[][] CreateTorusSegment(Matrix<double> transformation, double profileRadius, double sectionsRadius, double angle,
            int profileSize = 16, int sectionsCount = 5)
        {
            Vector<double>[][] points = new Vector<double>[sectionsCount][];

            for (int i = 0; i < sectionsCount; i++)
            {
                Vector<double>[] profilePoints = new Vector<double>[profileSize];
                double psi = angle * i / (sectionsCount - 1);
                for (int j = 0; j < profileSize; j++)
                {
                    double phi = 2 * Math.PI * j / profileSize;
                    double x = (sectionsRadius + profileRadius * Math.Cos(psi)) * Math.Cos(phi);
                    double y = (sectionsRadius + profileRadius * Math.Cos(psi)) * Math.Sin(phi);
                    double z = profileRadius * Math.Sin(psi);
                    Vector<double> point = new DenseVector(new[]
                    {
                        x, y, z
                    });

                    profilePoints[j] = transformation.GetRotation().Multiply(point) + transformation.GetOffset();
                }

                points[i] = profilePoints;
            }

            return points;
        }

        [Pure]
        public static Vector<double>[] CreateCircle(Vector<double> origin, double radius, Vector<double> direction, Vector<double> refDirection,
            int numSegments = 16)
        {
            return CreateArc(origin, radius, 2 * Math.PI, direction, refDirection, numSegments);
        }

        [Pure]
        public static Vector<double>[] CreateArc(Vector<double> origin, double radius, double angle, Vector<double> direction, Vector<double> refDirection,
            int numSegments = 8, bool endPoint = false)
        {
            Vector<double> directionNorm = direction.Normalize(2);
            Vector<double> refDirectionNorm = refDirection.Normalize(2);
            Vector<double> upDirectionNorm = directionNorm.CrossProduct(refDirectionNorm).Normalize(2);

            Matrix<double> transform = MatrixExtensions.CreateTransition(origin, refDirectionNorm, upDirectionNorm, directionNorm).GetRotation();
            Matrix<double> transformInv = transform.Inverse();

            Vector<double>[] localPoints = new Vector<double>[numSegments];

            int divFactor = endPoint ? numSegments - 1 : numSegments;
            for (int i = 0; i < numSegments; i++)
            {
                double t = angle * i / divFactor;
                double x = radius * Math.Cos(t);
                double y = radius * Math.Sin(t);
                localPoints[i] = new DenseVector(new[]
                {
                    x, y, 0.0
                });
            }

            Vector<double>[] worldPoints = localPoints.Select(point => transformInv.Multiply(point) + origin).ToArray();
            return worldPoints;
        }
    }
}