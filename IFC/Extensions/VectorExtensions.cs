using System;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Extensions
{
    public static class VectorExtensions
    {
        private const double TOLERANCE = 1e-3;

        public static XbimVector3D X => new XbimVector3D(1, 0, 0);
        public static XbimVector3D Y => new XbimVector3D(0, 1, 0);
        public static XbimVector3D Z => new XbimVector3D(0, 0, 1);
        
        public static XbimVector3D Right => X;
        public static XbimVector3D Up => Y;
        public static XbimVector3D Forward => Z;
    
        public static double SignedAngle(this XbimVector3D first, XbimVector3D second)
        {
            return Math.Acos(XbimVector3D.DotProduct(first, second) / (first.Length * second.Length));
        }

        public static bool IsParallel(this XbimVector3D v1, XbimVector3D v2)
        {
            return Math.Abs(1 / (v1.Length * v2.Length) * Math.Abs(XbimVector3D.DotProduct(v1, v2)) - 1) < TOLERANCE;
        }
        
        public static IfcDirection ToIfcDirection(this XbimVector3D vector, IModel model)
        {
            return IfcAxis.CreateDirection(model, vector);
        }

        public static IfcCartesianPoint ToCartesianPoint(this XbimVector3D vector, IModel model)
        {
            return IfcAxis.CreatePoint(model, vector);
        }

        public static XbimVector3D RotateAroundXAxis(this XbimVector3D vector3D, double angle)
        {
            XbimMatrix3D Mx = MatrixExtensions.Mx(angle);
            return XbimVector3D.Multiply(vector3D, Mx);
        }
        
        public static XbimVector3D RotateAroundYAxis(this XbimVector3D vector3D, double angle)
        {
            XbimMatrix3D My = MatrixExtensions.My(angle);
            return XbimVector3D.Multiply(vector3D, My);
        }
        
        public static XbimVector3D RotateAroundZAxis(this XbimVector3D vector3D, double angle)
        {
            XbimMatrix3D Mz = MatrixExtensions.Mz(angle);
            return XbimVector3D.Multiply(vector3D, Mz);
        }
    }
}