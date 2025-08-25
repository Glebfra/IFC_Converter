using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Extensions
{
    public static class IfcCartesianPointExtensions
    {
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
    }
}