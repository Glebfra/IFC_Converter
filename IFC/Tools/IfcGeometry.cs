using System.Collections.Generic;
using System.Linq;
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
            return model.Instances.New<IfcArbitraryClosedProfileDef>(def =>
            {
                def.ProfileType = IfcProfileTypeEnum.AREA;
                def.OuterCurve = curve;
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

        public static IfcCircle CreateCircle(IModel model, double radius, XbimVector3D coordinates, XbimVector3D direction, XbimVector3D refDirection)
        {
            return model.Instances.New<IfcCircle>(ifcCircle =>
            {
                ifcCircle.Radius = radius;
                ifcCircle.Position = IfcAxis.CreateAxis2Placement3D(model, coordinates, direction, refDirection);
            });
        }
        
        public static IfcCircle CreateCircle(IModel model, double radius, XbimVector3D coordinates)
        {
            return model.Instances.New<IfcCircle>(ifcCircle =>
            {
                ifcCircle.Radius = radius;
                ifcCircle.Position = IfcAxis.CreateAxis2Placement3D(model, coordinates);
            });
        }

        public static IfcCircleProfileDef CreateCircleProfileDef(IModel model, double radius, XbimVector3D coordinates, XbimVector3D direction)
        {
            return model.Instances.New<IfcCircleProfileDef>(def =>
            {
                def.ProfileType = IfcProfileTypeEnum.AREA;
                def.Radius = radius;
                def.Position = IfcAxis.CreateAxis2Placement2D(model, coordinates, direction);
            });
        }

        public static IfcCircleProfileDef CreateCircleProfileDef(IModel model, double radius, XbimVector3D coordinates)
        {
            return model.Instances.New<IfcCircleProfileDef>(def =>
            {
                def.ProfileType = IfcProfileTypeEnum.AREA;
                def.Radius = radius;
                def.Position = IfcAxis.CreateAxis2Placement2D(model, coordinates);
            });
        }

        public static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IfcRepresentationItem representationItem)
        {
            return model.Instances.New<IfcShapeRepresentation>(sr =>
            {
                sr.ContextOfItems = model.Instances.FirstOrDefault<IfcGeometricRepresentationContext>();
                sr.RepresentationIdentifier = "Body";
                sr.RepresentationType = "SweptSolid";
                sr.Items.Add(representationItem);
            });
        }

        public static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IEnumerable<IfcRepresentationItem> representationItems)
        {
            return model.Instances.New<IfcShapeRepresentation>(sr =>
            {
                sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
                sr.RepresentationIdentifier = "Body";
                sr.RepresentationType = "SweptSolid";
                sr.Items.AddRange(representationItems);
            });
        }

        public static IfcProductDefinitionShape CreateProductDefinitionShape(IModel model, IfcShapeRepresentation shapeRepresentation)
        {
            return model.Instances.New<IfcProductDefinitionShape>(shape => shape.Representations.Add(shapeRepresentation));
        }
    }
}