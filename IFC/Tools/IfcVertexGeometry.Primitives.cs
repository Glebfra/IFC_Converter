using System;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Tools
{
    public static partial class IfcVertexGeometry
    {
        public static IfcFacetedBrep CreateCone(IModel model, double radius, double height, XbimVector3D coordinates, int numSegments, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            xAxis = xAxis.Normalized();
            yAxis = yAxis.Normalized();
            XbimVector3D zAxis = XbimVector3D.CrossProduct(xAxis, yAxis).Normalized();
            
            IfcCartesianPoint[] botCircle = CreateCircle(model, radius, coordinates, numSegments, xAxis, yAxis);
            IfcCartesianPoint topPoint = (coordinates + zAxis * height).ToCartesianPoint(model);
            
            return IfcVertexGeometry.CreateCone(model, botCircle, topPoint);
        }

        public static IfcFacetedBrep CreateClippedCone(IModel model, double botRadius, double topRadius, double height, XbimVector3D coordinates, int numSegments, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            xAxis = xAxis.Normalized();
            yAxis = yAxis.Normalized();
            XbimVector3D zAxis = XbimVector3D.CrossProduct(xAxis, yAxis).Normalized();
            XbimVector3D topCoordinates = coordinates + height * zAxis;
            
            IfcCartesianPoint[] botCircle = IfcVertexGeometry.CreateCircle(model, botRadius, coordinates, numSegments, xAxis, yAxis);
            IfcCartesianPoint[] topCircle = IfcVertexGeometry.CreateCircle(model, topRadius, topCoordinates, numSegments, xAxis, yAxis);

            return IfcVertexGeometry.CreateClippedCone(model, botCircle, topCircle);
        }

        public static IfcFacetedBrep CreateSphere(IModel model, double radius, XbimVector3D coordinates, int numSegments, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            IfcCartesianPoint[,] spherePoints = IfcVertexGeometry.CreateSpherePoints(model, radius, coordinates, numSegments, xAxis, yAxis);
            return IfcVertexGeometry.CreateSphere(model, spherePoints);
        }

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
            faces[facesIndex] = CreatePolygonFace(model, points);
            
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

        public static IfcFacetedBrep CreateSphere(IModel model, IfcCartesianPoint[,] points)
        {
            int length1 = points.GetLength(0);
            int length2 = points.GetLength(1);

            IfcFace[] faces = new IfcFace[length1 * length2];
            int facesIndex = 0;
            for (int i = 0; i < length1; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    IfcCartesianPoint p1 = points[i, j];
                    IfcCartesianPoint p2 = points[i, (j + 1) % length2];
                    IfcCartesianPoint p3 = points[(i + 1) % length1, (j + 1) % length2];
                    IfcCartesianPoint p4 = points[(i + 1) % length1, j];
                    faces[facesIndex++] = CreateRectangleFace(model, p1, p2, p3, p4);
                }
            }
            
            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }
        
        public static IfcCartesianPoint[,] CreateSpherePoints(IModel model, double radius, XbimVector3D coordinates, int numSegments, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            xAxis = xAxis.Normalized();
            yAxis = yAxis.Normalized();
            XbimVector3D zAxis = XbimVector3D.CrossProduct(xAxis, yAxis).Normalized();
            
            double angleStep = 2 * Math.PI / numSegments;
            
            IfcCartesianPoint[,] points = new IfcCartesianPoint[numSegments, numSegments];
            for (int i = 0; i < numSegments; i++)
            {
                for (int j = 0; j < numSegments; j++)
                {
                    double x = radius * Math.Cos(angleStep * i) * Math.Cos(angleStep * j);
                    double y = radius * Math.Cos(angleStep * i) * Math.Sin(angleStep * j);
                    double z = radius * Math.Sin(angleStep * i);
                    points[i, j] = (x * xAxis + y * yAxis + z * zAxis).ToCartesianPoint(model);
                }
            }

            return points;
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
    }
}