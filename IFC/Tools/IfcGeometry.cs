using System.Collections.Generic;
using System.Linq;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Tools
{
    public static class IfcGeometry
    {
        public static IfcArbitraryClosedProfileDef CreateProfile(IModel model, IfcCurve curve)
        {
            return model.Instances.New<IfcArbitraryClosedProfileDef>(profileDef =>
            {
                profileDef.ProfileType = IfcProfileTypeEnum.AREA;
                profileDef.OuterCurve = curve;
            });
        }
        
        public static IfcPlane CreatePlane(IModel model, XbimVector3D coordinates, XbimVector3D direction)
        {
            return model.Instances.New<IfcPlane>(plane =>
            {
                plane.Position = IfcAxis.CreateAxis2Placement3D(model, coordinates, direction);
            });
        }

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

        public static IfcSweptDiskSolid CreateSweptDiskSolid(IModel model, IfcCurve curve, IfcPositiveLengthMeasure radius)
        {
            return model.Instances.New<IfcSweptDiskSolid>(solid =>
            {
                solid.Directrix = curve;
                solid.Radius = radius;
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

        public static IfcExtrudedAreaSolid CreateRectangle(IModel model, double xDim, double yDim, double zDim, XbimVector3D coordinates)
        {
            IfcRectangleProfileDef rectangleProfileDef = CreateRectangleProfileDef(model, xDim, yDim);
            return CreateExtrudedAreaSolid(model, rectangleProfileDef, zDim, coordinates);
        }

        public static IfcExtrudedAreaSolid CreateCylinder(IModel model, double radius, double zDim, XbimVector3D coordinates)
        {
            IfcCircleProfileDef circleProfileDef = CreateCircleProfileDef(model, radius);
            return CreateExtrudedAreaSolid(model, circleProfileDef, zDim, coordinates);
        }
        
        public static IfcExtrudedAreaSolid CreateRectangle(IModel model, double xDim, double yDim, double zDim, XbimVector3D coordinates, XbimVector3D axis, XbimVector3D refDirection)
        {
            IfcRectangleProfileDef rectangleProfileDef = CreateRectangleProfileDef(model, xDim, yDim);
            return CreateExtrudedAreaSolid(model, rectangleProfileDef, zDim, coordinates, axis, refDirection);
        }

        public static IfcExtrudedAreaSolid CreateCylinder(IModel model, double radius, double zDim, XbimVector3D coordinates, XbimVector3D axis, XbimVector3D refDirection)
        {
            IfcCircleProfileDef circleProfileDef = CreateCircleProfileDef(model, radius);
            return CreateExtrudedAreaSolid(model, circleProfileDef, zDim, coordinates, axis, refDirection);
        }

        public static IfcExtrudedAreaSolid CreateExtrudedAreaSolid(IModel model, IfcProfileDef profileDef, double zDim, XbimVector3D coordinates)
        {
            IfcCartesianPoint point = IfcAxis.CreatePoint(model, coordinates);
            IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point);
            IfcDirection extrudeDirection = IfcAxis.CreateDirection(model, VectorExtensions.Forward);

            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Depth = zDim;
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = extrudeDirection;
                solid.Position = axis2Placement3D;
            });
        }
        
        public static IfcExtrudedAreaSolid CreateExtrudedAreaSolid(IModel model, IfcProfileDef profileDef, double zDim, XbimVector3D coordinates, XbimVector3D axis, XbimVector3D refDirection)
        {
            IfcCartesianPoint point = IfcAxis.CreatePoint(model, coordinates);
            IfcDirection ifcAxis = IfcAxis.CreateDirection(model, axis);
            IfcDirection ifcRefDirection = IfcAxis.CreateDirection(model, refDirection);
            IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point, ifcAxis, ifcRefDirection);
            IfcDirection extrudeDirection = IfcAxis.CreateDirection(model, VectorExtensions.Forward);

            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Depth = zDim;
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = extrudeDirection;
                solid.Position = axis2Placement3D;
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

        public static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IfcRepresentationItem representationItem)
        {
            return CreateShapeRepresentation(model, new[] { representationItem });
        }

        public static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IEnumerable<IfcRepresentationItem> representationItems)
        {
            return model.Instances.New<IfcShapeRepresentation>(representation =>
            {
                representation.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
                representation.RepresentationIdentifier = "Body";
                representation.RepresentationType = "SweptSolid";
                representation.Items.AddRange(representationItems);
            });
        }

        public static IfcProductDefinitionShape CreateProductDefinitionShape(IModel model, IfcShapeRepresentation shapeRepresentation)
        {
            return model.Instances.New<IfcProductDefinitionShape>(shape => shape.Representations.Add(shapeRepresentation));
        }
    }
}