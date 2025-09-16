using System;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.ProfileResource;

namespace IFC.Extensions
{
    public static class IfcExtrudedAreaSolidExtensions
    {
        public static XbimVector3D[] GetBoundPoints(this IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            XbimVector3D[] points = new XbimVector3D[2];
            
            XbimMatrix3D areaSolidMatrix3D = extrudedAreaSolid.Position.ToMatrix3D();
            XbimVector3D forward = extrudedAreaSolid.ExtrudedDirection.XbimVector3D();
            double length = GetLength(extrudedAreaSolid);
            
            XbimVector3D internalSecondPoint = forward * length;

            XbimVector3D globalFirstPoint = areaSolidMatrix3D.Translation;
            XbimVector3D globalSecondPoint = globalFirstPoint + areaSolidMatrix3D.Transform(internalSecondPoint);

            return new XbimVector3D[] { globalFirstPoint, globalSecondPoint };

            points[0] = areaSolidMatrix3D.Translation;
            points[1] = points[0] + forward * length;
            
            return points;
        }

        public static double GetLength(this IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            return extrudedAreaSolid.Depth;
        }

        public static double GetCircleRadius(this IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            if (extrudedAreaSolid.SweptArea is IfcCircleProfileDef circleProfileDef)
            {
                return circleProfileDef.Radius;
            }

            throw new ArgumentException($"{nameof(extrudedAreaSolid)} does not contain {nameof(IfcCircleProfileDef)}");
        }
    }
}