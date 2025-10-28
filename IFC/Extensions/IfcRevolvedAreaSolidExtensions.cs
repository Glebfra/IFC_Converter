using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.ProfileResource;

namespace IFC.Extensions
{
    public static class IfcRevolvedAreaSolidExtensions
    {
        public static BendProperties GetBendProperties(this IfcRevolvedAreaSolid revolvedAreaSolid)
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
            XbimVector3D globalAxisLocation = areaSolidMatrix3D.Translation + internalAxisLocation;
            
            XbimVector3D[] boundPoints = new XbimVector3D[] { globalFirstPoint, globalSecondPoint };

            double pipeDiameter = revolvedAreaSolid.SweptArea is IfcCircleProfileDef circleProfileDef ? circleProfileDef.Radius * 2 : 0;

            return new BendProperties()
            {
                Angle = angle,
                BoundPoints = boundPoints,
                Center = globalAxisLocation,
                Radius = internalAxisLocation.Length,
                PipeDiameter = pipeDiameter
            };
        }
    }
}