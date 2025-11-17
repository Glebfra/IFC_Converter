using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Extensions
{
    public static class IfcAxisExtensions
    {
        public static XbimMatrix3D ToObjectMatrix3D(this IIfcAxis2Placement3D axis2Placement3D)
        {
            XbimMatrix3D objectMatrix3D = axis2Placement3D.ToMatrix3D();
            objectMatrix3D = XbimMatrix3D.CreateWorld(objectMatrix3D.Translation, objectMatrix3D.Backward, objectMatrix3D.Up);
            return objectMatrix3D;
        }
        
        public static XbimMatrix3D ToObjectMatrix3D(this IIfcObjectPlacement objectPlacement)
        {
            XbimMatrix3D objectMatrix3D = objectPlacement.ToMatrix3D();
            objectMatrix3D = XbimMatrix3D.CreateWorld(objectMatrix3D.Translation, objectMatrix3D.Backward, objectMatrix3D.Up);
            return objectMatrix3D;
        }

        public static IfcCartesianPointList3D CreateCartesianPointList3D(IModel model, XbimVector3D[] points)
        {
            return model.Instances.New<IfcCartesianPointList3D>(list3D =>
            {
                for (int i = 0; i < points.Length; i++)
                {
                    list3D.CoordList.GetAt(i).AddRange(new IfcLengthMeasure[] { points[i].X, points[i].Y, points[i].Z });
                }
            });
        }

        public static IfcCartesianPointList2D CreateCartesianPointList2D(IModel model, XbimVector3D[] points)
        {
            return model.Instances.New<IfcCartesianPointList2D>(list2D =>
            {
                for (int i = 0; i < points.Length; i++)
                {
                    list2D.CoordList.GetAt(i).AddRange(new IfcLengthMeasure[] { points[i].X, points[i].Y });
                }
            });
        }

        public static IEnumerable<XbimVector3D> GetCoordinates(this IfcCartesianPointList3D pointList3D)
        {
            return pointList3D.CoordList.Select(coords => new XbimVector3D(coords[0], coords[1], coords[2]));
        }

        public static void RotateAroundXAxis(this IfcCartesianPoint point, double angle)
        {
            XbimVector3D vector3D = ToXbimVector3D(point);
            vector3D = vector3D.RotateAroundXAxis(angle);
            point.SetXYZ(vector3D.X, vector3D.Y, vector3D.Z);
        }
        
        public static void RotateAroundYAxis(this IfcCartesianPoint point, double angle)
        {
            XbimVector3D vector3D = ToXbimVector3D(point);
            vector3D = vector3D.RotateAroundYAxis(angle);
            point.SetXYZ(vector3D.X, vector3D.Y, vector3D.Z);
        }
        
        public static void RotateAroundZAxis(this IfcCartesianPoint point, double angle)
        {
            XbimVector3D vector3D = ToXbimVector3D(point);
            vector3D = vector3D.RotateAroundZAxis(angle);
            point.SetXYZ(vector3D.X, vector3D.Y, vector3D.Z);
        }

        public static XbimVector3D ToXbimVector3D(this IfcCartesianPoint point)
        {
            return new XbimVector3D(point.X, point.Y, point.Z);
        }

        public static void SetVector(this IfcCartesianPoint point, XbimVector3D vector3D)
        {
            point.SetXYZ(vector3D.X, vector3D.Y, vector3D.Z);
        }
        
        public static void SetVector(this IfcDirection direction, XbimVector3D vector3D)
        {
            direction.SetXYZ(vector3D.X, vector3D.Y, vector3D.Z);
        }
    }
}