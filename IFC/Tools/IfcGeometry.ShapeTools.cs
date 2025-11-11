using System.Collections.Generic;
using System.Linq;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Tools
{
    public static partial class IfcGeometry
    {
        public static IfcShapeRepresentation CreateShapeRepresentation(
            IModel model, IfcRepresentationItem representationItem, 
            string representationType = IfcRepresentationType.SweptSolid, string representationIdentifier = IfcRepresentationIdentifier.Body
        ) => CreateShapeRepresentation(model, new[] { representationItem }, representationType, representationIdentifier);

        public static IfcShapeRepresentation CreateShapeRepresentation(
            IModel model, IEnumerable<IfcRepresentationItem> representationItems, 
            string representationType = IfcRepresentationType.SweptSolid, string representationIdentifier = IfcRepresentationIdentifier.Body
        )
        {
            return model.Instances.New<IfcShapeRepresentation>(representation =>
            {
                representation.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
                representation.RepresentationIdentifier = representationIdentifier;
                representation.RepresentationType = representationType;
                representation.Items.AddRange(representationItems);
            });
        }
        
        public static IfcFace CreateRectangleFace(IModel model, IfcCartesianPoint p1, IfcCartesianPoint p2, IfcCartesianPoint p3, IfcCartesianPoint p4)
        {
            return CreatePolygonFace(model, new[] { p1, p2, p3, p4 });
        }
        
        public static IfcFace CreateTriangleFace(IModel model, IfcCartesianPoint p1, IfcCartesianPoint p2, IfcCartesianPoint p3)
        {
            return CreatePolygonFace(model, new[] { p1, p2, p3 });
        }
        
        public static IfcFace CreatePolygonFace(IModel model, IEnumerable<IfcCartesianPoint> points)
        {
            return model.Instances.New<IfcFace>(f =>
            {
                f.Bounds.Add(model.Instances.New<IfcFaceOuterBound>(b =>
                {
                    b.Bound = model.Instances.New<IfcPolyLoop>(pl =>
                    {
                        pl.Polygon.AddRange(points);
                    });
                    b.Orientation = true;
                }));
            });
        }
        
        public static IfcPolyline CreatePolyline(IModel model, IEnumerable<IfcCartesianPoint> points)
        {
            return model.Instances.New<IfcPolyline>(polyline =>
            {
                polyline.Points.AddRange(points);
            });
        }

        public static IfcProductDefinitionShape CreateProductDefinitionShape(IModel model, IfcShapeRepresentation shapeRepresentation)
        {
            return model.Instances.New<IfcProductDefinitionShape>(shape => shape.Representations.Add(shapeRepresentation));
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
        
        public static IfcSweptDiskSolid CreateSweptDiskSolid(IModel model, IfcCurve curve, IfcPositiveLengthMeasure radius)
        {
            return model.Instances.New<IfcSweptDiskSolid>(solid =>
            {
                solid.Directrix = curve;
                solid.Radius = radius;
            });
        }
    }
}