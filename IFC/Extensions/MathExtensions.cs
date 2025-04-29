using System;

namespace IFC.Extensions
{
    public static class MathExtensions
    {
        public static double CalculateSphereArea(double radius)
        {
            return 4 * Math.PI * radius * radius;
        }

        public static double CalculateCylinderArea(double radius, double height)
        {
            return 2 * Math.PI * height * radius;
        }

        public static double CalculateClippedConeArea(double firstRadius, double secondRadius, double height)
        {
            double radius = secondRadius - firstRadius;
            double sideLength = Math.Sqrt(radius * radius + height * height);
            return Math.PI * sideLength * (firstRadius + secondRadius);
        }
    }
}