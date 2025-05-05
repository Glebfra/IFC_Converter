using System;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract;
using IFC.Tools;
using Xbim.Common.Geometry;

namespace IFC.Extensions
{
    public static class MatrixExtensions
    {
        public static XbimMatrix3D Mx(double angle)
        {
            double angleCos = Math.Cos(angle);
            double angleSin = Math.Sin(angle);

            return new XbimMatrix3D(
                1, 0, 0, 0,
                0, angleCos, -angleSin, 0,
                0, angleSin, angleCos, 0,
                0, 0, 0, 1
            );
        }
    
        public static XbimMatrix3D My(double angle)
        {
            double angleCos = Math.Cos(angle);
            double angleSin = Math.Sin(angle);

            return new XbimMatrix3D(
                angleCos, 0, angleSin, 0,
                0, 1, 0, 0,
                -angleSin, 0, angleCos, 0,
                0, 0, 0, 1
            );
        }
    
        public static XbimMatrix3D Mz(double angle)
        {
            double angleCos = Math.Cos(angle);
            double angleSin = Math.Sin(angle);

            return new XbimMatrix3D(
                angleCos, -angleSin, 0, 0,
                angleSin, angleCos, 0, 0,
                -0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        public static XbimMatrix3D Translation(XbimVector3D translation)
        {
            return new XbimMatrix3D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                translation.X, translation.Y, translation.Z, 1
            );
        }

        public static XbimMatrix3D Translate(this XbimMatrix3D matrix3D, XbimVector3D translationVector)
        {
            matrix3D.OffsetX += translationVector.X;
            matrix3D.OffsetY += translationVector.Y;
            matrix3D.OffsetZ += translationVector.Z;
            return matrix3D;
        }
    }
}