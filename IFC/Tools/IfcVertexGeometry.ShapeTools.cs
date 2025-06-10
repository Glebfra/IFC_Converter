using System.Collections.Generic;
using System.Linq;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Tools
{
    public static partial class IfcVertexGeometry
    {
        public static IfcTriangulatedFaceSet CreateTriangulatedFaceSet(IModel model, IfcCartesianPointList3D coordinates, int[][] vertices, XbimVector3D[] normals, bool closed)
        {
            IfcTriangulatedFaceSet triangulatedFaceSet = model.Instances.New<IfcTriangulatedFaceSet>();
            triangulatedFaceSet.Coordinates = coordinates;
            triangulatedFaceSet.Closed = closed;

            int vertexIndex = 0;
            triangulatedFaceSet.AddVertices(vertices, ref vertexIndex);
            int normalIndex = 0;
            triangulatedFaceSet.AddNormals(normals, ref normalIndex);

            return triangulatedFaceSet;
        }

        public static IfcPolygonalFaceSet CreatePolygonalFaceSet(IModel model, IfcCartesianPointList3D coordinates)
        {
            IfcPolygonalFaceSet polygonalFaceSet = model.Instances.New<IfcPolygonalFaceSet>();
            polygonalFaceSet.Coordinates = coordinates;

            return polygonalFaceSet;
        }
        
        public static IfcIndexedPolygonalFace CreateIndexedPolygonalFace(IModel model, int[] indices)
        {
            IfcIndexedPolygonalFace indexedFace = model.Instances.New<IfcIndexedPolygonalFace>();
            indexedFace.CoordIndex.AddRange(indices.Cast<IfcPositiveInteger>());
            return indexedFace;
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
    }
}