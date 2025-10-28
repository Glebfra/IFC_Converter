using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Extensions
{
    public static class VectorExtensions
    {
        public static XbimVector3D X => new XbimVector3D(1, 0, 0);
        public static XbimVector3D Y => new XbimVector3D(0, 1, 0);
        public static XbimVector3D Z => new XbimVector3D(0, 0, 1);
        
        public static XbimVector3D Right => X;
        public static XbimVector3D Up => Y;
        public static XbimVector3D Forward => Z;

        public static XbimVector3D GetNearestVector(this XbimVector3D first, params XbimVector3D[] others)
        {
            return others
                .Select(vector => vector - first)
                .OrderBy(vector => vector.Length)
                .First();
        }
        
        public static XbimVector3D Sum(this IEnumerable<XbimVector3D> vectors)
        {
            XbimVector3D[] xbimVector3Ds = vectors as XbimVector3D[] ?? vectors.ToArray();
            IEnumerable<double> xs = xbimVector3Ds.Select(v => v.X);
            IEnumerable<double> ys = xbimVector3Ds.Select(v => v.Y);
            IEnumerable<double> zs = xbimVector3Ds.Select(v => v.Z);
            
            return new XbimVector3D(xs.Sum(), ys.Sum(), zs.Sum());
        }

        public static XbimVector3D Average(this IEnumerable<XbimVector3D> vectors)
        {
            XbimVector3D[] xbimVector3Ds = vectors as XbimVector3D[] ?? vectors.ToArray();
            return 1.0 / xbimVector3Ds.Count() * xbimVector3Ds.Sum();
        }
        
        public static double GetDistance(this XbimVector3D first, XbimVector3D second)
        {
            return (second - first).Length;
        }

        public static double SignedAngle(this XbimVector3D first, XbimVector3D second)
        {
            return Math.Acos(XbimVector3D.DotProduct(first, second) / (first.Length * second.Length));
        }

        public static ActionProperty<XbimVector3D> CrossProduct(ActionProperty<XbimVector3D> first, ActionProperty<XbimVector3D> other)
        {
            ActionProperty<XbimVector3D> cross = XbimVector3D.CrossProduct(first, other);

            first.OnValueChange += () => cross.Value = XbimVector3D.CrossProduct(first, other);
            other.OnValueChange += () => cross.Value = XbimVector3D.CrossProduct(first, other);

            return cross;
        }

        public static bool IsParallel(this XbimVector3D v1, XbimVector3D v2, double tolerance = 1e-3)
        {
            return Math.Abs(1 / (v1.Length * v2.Length) * Math.Abs(XbimVector3D.DotProduct(v1, v2)) - 1) < tolerance;
        }

        public static bool IsEqualFixed(this XbimVector3D vector3D, XbimVector3D other, double precision = 1e-9)
        {
            return Math.Abs(vector3D.GetDistance(other)) <= precision;
        }

        public static bool IsEqualFixed(this XbimVector3D vector3D, IEnumerable<XbimVector3D> others, double precision = 1e-9)
        {
            return others.Any(vector => vector3D.IsEqualFixed(vector, precision));
        }

        public static bool IsEqualFixed(this IEnumerable<XbimVector3D> vector3Ds, IEnumerable<XbimVector3D> others, double precision = 1e-9)
        {
            return vector3Ds.Any(vector => vector.IsEqualFixed(others, precision));
        }

        public static bool IsEqual(this XbimVector3D vector3D, IEnumerable<XbimVector3D> vector3Ds, double precision = 1e-9)
        {
            return vector3Ds.Any(vector => vector.IsEqual(vector3D, precision));
        }

        public static IfcDirection ToIfcDirection(this XbimVector3D vector, IModel model)
        {
            return IfcAxis.CreateDirection(model, vector);
        }

        public static IfcCartesianPoint ToCartesianPoint(this XbimVector3D vector, IModel model)
        {
            return IfcAxis.CreatePoint(model, vector);
        }

        public static XbimVector3D RotateAroundAxis(this XbimVector3D vector3D, XbimVector3D axis, double angle)
        {
            XbimMatrix3D Ma = MatrixExtensions.Ma(axis, angle);
            return XbimVector3D.Multiply(vector3D, Ma);
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