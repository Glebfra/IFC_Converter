using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;

namespace IFC.Tools.Geometry
{
    public static partial class IfcGeometry
    {
        public static IfcTrimmedCurve CreateTrimmedCurve(IModel model, IfcCurve basicCurve, double firstParameter, double secondParameter)
        {
            return model.Instances.New<IfcTrimmedCurve>(curve =>
            {
                curve.BasisCurve = basicCurve;
                curve.Trim1.Add(new IfcParameterValue(firstParameter));
                curve.Trim2.Add(new IfcParameterValue(secondParameter));
                curve.SenseAgreement = true;
                curve.MasterRepresentation = IfcTrimmingPreference.PARAMETER;
            });
        }

        public static IfcCircle CreateCircle(IModel model, double radius, XbimVector3D coordinates, XbimVector3D direction, XbimVector3D refDirection)
        {
            return model.Instances.New<IfcCircle>(circle =>
            {
                circle.Radius = radius;
                circle.Position = IfcAxis.CreateAxis2Placement3D(model, coordinates, direction, refDirection);
            });
        }
        
        public static IfcCircle CreateCircle(IModel model, double radius, XbimVector3D coordinates)
        {
            return model.Instances.New<IfcCircle>(circle =>
            {
                circle.Radius = radius;
                circle.Position = IfcAxis.CreateAxis2Placement3D(model, coordinates);
            });
        }
        
        public static IfcArbitraryClosedProfileDef CreateProfile(IModel model, IfcCurve curve)
        {
            return model.Instances.New<IfcArbitraryClosedProfileDef>(profileDef =>
            {
                profileDef.ProfileType = IfcProfileTypeEnum.AREA;
                profileDef.OuterCurve = curve;
            });
        }
        
        public static IfcRectangleProfileDef CreateRectangleProfileDef(IModel model, double xDim, double yDim)
        {
            return model.Instances.New<IfcRectangleProfileDef>(def =>
            {
                def.ProfileType = IfcProfileTypeEnum.AREA;
                def.XDim = xDim;
                def.YDim = yDim;
            });
        }
        
        public static IfcCircleProfileDef CreateCircleProfileDef(IModel model, double radius, XbimVector3D coordinates, XbimVector3D direction)
        {
            return model.Instances.New<IfcCircleProfileDef>(profileDef =>
            {
                profileDef.ProfileType = IfcProfileTypeEnum.AREA;
                profileDef.Radius = radius;
                profileDef.Position = IfcAxis.CreateAxis2Placement2D(model, coordinates, direction);
            });
        }

        public static IfcCircleProfileDef CreateCircleProfileDef(IModel model, double radius, XbimVector3D coordinates)
        {
            return model.Instances.New<IfcCircleProfileDef>(profileDef =>
            {
                profileDef.ProfileType = IfcProfileTypeEnum.AREA;
                profileDef.Radius = radius;
                profileDef.Position = IfcAxis.CreateAxis2Placement2D(model, coordinates);
            });
        }
        
        public static IfcCircleProfileDef CreateCircleProfileDef(IModel model, double radius)
        {
            return model.Instances.New<IfcCircleProfileDef>(profileDef =>
            {
                profileDef.ProfileType = IfcProfileTypeEnum.AREA;
                profileDef.Radius = radius;
            });
        }
    }
}