using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
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
            // In XbimVector3D, rotation occurs along the left trio of vectors for some reason. Therefore, for correct calculations, we use the minus angle.
            XbimVector3D internalSecondPoint = internalFirstPoint.RotateAroundAxis(internalAxisDirection, -angle);

            XbimMatrix3D areaSolidMatrix3D = revolvedAreaSolid.Position.ToMatrix3D();
            XbimVector3D areaSolidDisplacement = areaSolidMatrix3D.Translation + areaSolidMatrix3D.Transform(internalAxisLocation);

            XbimVector3D globalFirstPoint = areaSolidMatrix3D.Transform(internalFirstPoint) + areaSolidDisplacement;
            XbimVector3D globalSecondPoint = areaSolidMatrix3D.Transform(internalSecondPoint) + areaSolidDisplacement;

            return new XbimVector3D[] { globalFirstPoint, globalSecondPoint };
        }

        public static XbimMatrix3D GetObjectMatrix(this IfcRevolvedAreaSolid revolvedAreaSolid)
        {
            return revolvedAreaSolid.Position.ToObjectMatrix3D();
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