using System;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.ProfileResource;

namespace IFC.Extensions
{
    public static class IfcExtrudedAreaSolidExtensions
    {
        [Obsolete("Use GetPipeProperties Instead")]
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
        }

        [Obsolete("Use GetPipeProperties Instead")]
        public static double GetLength(this IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            return extrudedAreaSolid.Depth;
        }

        public static PipeProperties GetPipeProperties(this IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            XbimMatrix3D areaSolidMatrix3D = extrudedAreaSolid.Position.ToMatrix3D();
            XbimVector3D forward = areaSolidMatrix3D.Transform(extrudedAreaSolid.ExtrudedDirection.XbimVector3D());
            double length = extrudedAreaSolid.Depth;
            double radius = GetCircleRadius(extrudedAreaSolid);
            
            XbimVector3D internalSecondPoint = forward * length;

            XbimVector3D globalFirstPoint = areaSolidMatrix3D.Translation;
            XbimVector3D globalSecondPoint = globalFirstPoint + areaSolidMatrix3D.Transform(internalSecondPoint);

            XbimVector3D[] boundPoints = new XbimVector3D[] { globalFirstPoint, globalSecondPoint };

            return new PipeProperties()
            {
                Radius = radius,
                BoundPoints = boundPoints,
                Direction = forward,
                Length = length,
                Coordinates = globalFirstPoint
            };
        }
        
        private static double GetCircleRadius(this IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            if (extrudedAreaSolid.SweptArea is IfcCircleProfileDef circleProfileDef)
            {
                return circleProfileDef.Radius;
            }

            throw new ArgumentException($"{nameof(extrudedAreaSolid)} does not contain {nameof(IfcCircleProfileDef)}");
        }
    }
}