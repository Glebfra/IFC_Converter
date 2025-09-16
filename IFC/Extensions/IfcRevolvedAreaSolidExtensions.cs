using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;

namespace IFC.Extensions
{
    public static class IfcRevolvedAreaSolidExtensions
    {
        public static XbimVector3D[] GetBoundPoints(this IfcRevolvedAreaSolid revolvedAreaSolid)
        {
            XbimVector3D internalAxisLocation = revolvedAreaSolid.Axis.Location.ToXbimVector3D();
            XbimVector3D internalAxisDirection = revolvedAreaSolid.Axis.Axis.XbimVector3D();

            double angle = revolvedAreaSolid.Angle;
            XbimVector3D internalFirstPoint = internalAxisLocation.Negated();
            XbimVector3D internalSecondPoint = internalFirstPoint.RotateAroundAxis(internalAxisDirection, angle);

            XbimMatrix3D areaSolidMatrix3D = revolvedAreaSolid.Position.ToObjectMatrix3D();
            XbimVector3D areaSolidDisplacement = areaSolidMatrix3D.Translation + areaSolidMatrix3D.Transform(internalAxisLocation);

            XbimVector3D globalFirstPoint = areaSolidMatrix3D.Transform(internalFirstPoint) + areaSolidDisplacement;
            XbimVector3D globalSecondPoint = areaSolidMatrix3D.Transform(internalSecondPoint) + areaSolidDisplacement;

            return new XbimVector3D[] { globalFirstPoint, globalSecondPoint };
        }

        public static double GetAngle(this IfcRevolvedAreaSolid revolvedAreaSolid)
        {
            return revolvedAreaSolid.Angle;
        }

        public static double GetRadius(this IfcRevolvedAreaSolid revolvedAreaSolid)
        {
            return revolvedAreaSolid.Axis.Location.ToXbimVector3D().Length;
        }
    }
}