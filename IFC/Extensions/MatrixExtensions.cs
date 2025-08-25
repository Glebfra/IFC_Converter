using System;
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

        public static XbimVector3D Offset(this XbimMatrix3D matrix3D)
        {
            return new XbimVector3D(matrix3D.OffsetX, matrix3D.OffsetY, matrix3D.OffsetZ);
        }

        public static XbimMatrix3D CreateWorld(XbimVector3D translation, XbimVector3D forward)
        {
            forward = forward.Normalized();
            XbimVector3D worldUp = forward.IsParallel(VectorExtensions.Z) ? VectorExtensions.Y : VectorExtensions.Z;
            XbimVector3D right = XbimVector3D.CrossProduct(forward, worldUp).Normalized();
            XbimVector3D up = XbimVector3D.CrossProduct(forward, right).Normalized();
            return XbimMatrix3D.CreateWorld(translation, forward, up);
        }

        public static XbimMatrix3D Inverted(this XbimMatrix3D matrix3D)
        {
            if (matrix3D.M44 == 0)
                matrix3D.M44 = 1;
            matrix3D.Invert();
            return matrix3D;
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