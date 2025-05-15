using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Tools
{
    public static partial class IfcVertexGeometry
    {
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
        
        public static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IfcRepresentationItem representationItem)
        {
            return CreateShapeRepresentation(model, new[] { representationItem });
        }

        public static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IEnumerable<IfcRepresentationItem> representationItems)
        {
            return model.Instances.New<IfcShapeRepresentation>(sr =>
            {
                sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
                sr.RepresentationIdentifier = "Body";
                sr.RepresentationType = "Brep";
                sr.Items.AddRange(representationItems);
            });
        }
        
        public static IfcPolyline CreatePolyline(IModel model, IEnumerable<IfcCartesianPoint> points)
        {
            return model.Instances.New<IfcPolyline>(polyline =>
            {
                polyline.Points.AddRange(points);
            });
        }
    }
}