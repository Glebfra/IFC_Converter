using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;

namespace IFC.Extensions
{
    public static class IfcTriangulatedFaceSetExtensions
    {
        public static double GetTorusRadius(this IfcTriangulatedFaceSet faceSet, double circleRadius)
        {
            XbimVector3D[] vertices = faceSet.Coordinates.GetCoordinates().ToArray();

            XbimVector3D point = vertices[0];
            double xx = point.X * point.X;
            double yy = point.Y * point.Y;
            double zz = point.Z * point.Z;
            double temp = (xx + yy + zz - circleRadius * circleRadius);
            double a = 1;
            double b = -2 * temp - 4 * (xx + yy);
            double c = temp * temp;

            double D = b * b - 4 * a * c;
            double sqrtD = Math.Sqrt(D);
            
            List<double> roots = new List<double>();
            if (D < 0)
                throw new ArgumentException("Cannot compute torus radius from given face set.");
            if (D == 0)
            {
                roots.Add(-b / (2 * a));
            }

            if (D > 0)
            {
                roots.Add((-b + sqrtD) / (2 * a));
                roots.Add((-b - sqrtD) / (2 * a));
            }
            
            if (roots.Count == 1 && roots[0] > 0)
                return Math.Sqrt(roots[0]);
            if (roots.Count == 2)
            {
                if (roots[0] > 0 && roots[1] > 0)
                    return Math.Max(Math.Sqrt(roots[0]), Math.Sqrt(roots[1]));
                if (roots[0] > 0)
                    return Math.Sqrt(roots[0]);
                if (roots[1] > 0)
                    return Math.Sqrt(roots[1]);
            }
            
            throw new ArgumentException("Cannot compute torus radius from given face set.");
        }
    }
}