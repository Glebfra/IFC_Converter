using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Tools
{
    public static class IfcVertexGeometry
    {
        public static IfcFacetedBrep CreateCone(IModel model, IfcCartesianPoint[] points, IfcCartesianPoint topPoint)
        {
            int numSegments = points.Length;
            IfcFace[] faces = new IfcFace[numSegments + 1];
            int facesIndex = 0;
            for (int i = 0; i < numSegments; i++)
            {
                IfcCartesianPoint p1 = points[i];
                IfcCartesianPoint p2 = points[(i + 1) % numSegments];
                faces[facesIndex++] = CreateTriangleFace(model, p1, p2, topPoint);
            }
            faces[facesIndex++] = CreatePolygonFace(model, points);
            
            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }
        
        public static IfcFacetedBrep CreateClippedCone(IModel model, IfcCartesianPoint[] points1, IfcCartesianPoint[] points2)
        {
            int numSegments = points1.Length;
            IfcFace[] faces = new IfcFace[numSegments + 2];
            int facesIndex = 0;
            for (int i = 0; i < numSegments; i++)
            {
                IfcCartesianPoint p1 = points1[i];
                IfcCartesianPoint p2 = points1[(i + 1) % numSegments];
                IfcCartesianPoint p3 = points2[(i + 1) % numSegments];
                IfcCartesianPoint p4 = points2[i];
                faces[facesIndex++] = CreateRectangleFace(model, p1, p2, p3, p4);
            }
            faces[facesIndex++] = CreatePolygonFace(model, points1);
            faces[facesIndex] = CreatePolygonFace(model, points2);
            
            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }

        public static IfcCartesianPoint[] CreateCircle(IModel model, double radius, XbimVector3D coordinates, int numSegments)
        {
            double angleStep = 2 * Math.PI / numSegments;
            
            IfcCartesianPoint[] points = new IfcCartesianPoint[numSegments];
            for (int i = 0; i < numSegments; i++)
            {
                XbimVector3D point = new XbimVector3D(radius * Math.Cos(angleStep * i), radius * Math.Sin(angleStep * i), 0);
                points[i] = IfcAxis.CreatePoint(model, point + coordinates);
            }

            return points;
        }
        
        public static IfcCartesianPoint[] CreateCircle(IModel model, double radius, XbimVector3D coordinates, int numSegments, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            double angleStep = 2 * Math.PI / numSegments;
            
            IfcCartesianPoint[] points = new IfcCartesianPoint[numSegments];
            for (int i = 0; i < numSegments; i++)
            {
                XbimVector3D x = xAxis * radius * Math.Cos(angleStep * i);
                XbimVector3D y = yAxis * radius * Math.Sin(angleStep * i);
                XbimVector3D point = x + y;
                points[i] = IfcAxis.CreatePoint(model, point + coordinates);
            }

            return points;
        }

        public static IfcCartesianPoint[] CreateSpiral(IModel model, double radius, double height, int numSegments, int numTurns, XbimVector3D displacement)
        {
            double pitch = height / numTurns;
            
            IfcCartesianPoint[] points = new IfcCartesianPoint[numTurns * numSegments];
            for (int i = 0; i < numTurns * numSegments; i++)
            {
                double factor = i / (double)numSegments;
                double angle = 2 * Math.PI * factor;
                
                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);
                double z = pitch * factor;
                
                XbimVector3D point = new XbimVector3D(x, y, z) + displacement;
                points[i] = IfcAxis.CreatePoint(model, point);
            }

            return points;
        }

        public static IfcPolyline CreatePolyline(IModel model, IEnumerable<IfcCartesianPoint> points)
        {
            return model.Instances.New<IfcPolyline>(polyline =>
            {
                polyline.Points.AddRange(points);
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
    }
}