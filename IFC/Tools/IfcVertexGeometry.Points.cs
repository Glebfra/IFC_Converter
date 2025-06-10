using System;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Tools
{
    public static partial class IfcVertexGeometry
    {
        public static IfcCartesianPointList3D CreateCircle(IModel model, double radius, IfcAxisSettings axisSettings, int numSegments)
        {
            double angleStep = 2 * Math.PI / numSegments;

            IfcCartesianPointList3D pointList3D = model.Instances.New<IfcCartesianPointList3D>();
            XbimVector3D[] points = new XbimVector3D[numSegments];
            for (int i = 0; i < numSegments; i++)
            {
                double x = radius * Math.Cos(i * angleStep);
                double y = radius * Math.Sin(i * angleStep);
                points[i] = axisSettings.XAxis * x + axisSettings.YAxis * y;
            }

            int pointIndex = 0;
            pointList3D.AddCoords(points, ref pointIndex);

            return pointList3D;
        }
        
        public static (IfcCartesianPointList3D, int[][], XbimVector3D[] normals) CreateSpherePoints(IModel model, double radius, IfcAxisSettings axisSettings, int numSegments)
        {
            double angleStep = 2 * Math.PI / numSegments;
            int numSegmentsSquared = numSegments * numSegments;
            
            IfcCartesianPointList3D pointList3D = model.Instances.New<IfcCartesianPointList3D>();
            XbimVector3D[] points = new XbimVector3D[numSegmentsSquared];
            int[][] vertices = new int[2 * numSegmentsSquared][];

            for (int i = 0; i < numSegments; i++)
            {
                for (int j = 0; j < numSegments; j++)
                {
                    double phi = angleStep * i;
                    double theta = angleStep * j;
                    double x = radius * Math.Cos(phi) * Math.Cos(theta);
                    double y = radius * Math.Cos(phi) * Math.Sin(theta);
                    double z = radius * Math.Sin(phi);
                    
                    int index = i * numSegments + j;
                    points[index] = axisSettings.XAxis * x + axisSettings.YAxis * y + axisSettings.ZAxis * z;
                    vertices[index * 2] = new int[] { index + 1, (index + numSegments) % numSegmentsSquared + 1, (index + numSegments + 1) % numSegmentsSquared + 1 };
                    vertices[index * 2 + 1] = new int[] { index + 1, (index + 1) % numSegmentsSquared + 1, (index + numSegments + 1) % numSegmentsSquared + 1 };
                }
            }

            XbimVector3D[] normals = new XbimVector3D[2 * numSegmentsSquared];
            for (int i = 0; i < 2 * numSegmentsSquared; i++)
            {
                XbimVector3D first = points[vertices[i][1] - 1] - points[vertices[i][0] - 1];
                XbimVector3D second = points[vertices[i][2] - 1] - points[vertices[i][1] - 1];
                normals[i] = XbimVector3D.CrossProduct(first, second).Normalized();
            }
            
            int pointIndex = 0;
            pointList3D.AddCoords(points, ref pointIndex);

            return (pointList3D, vertices, normals);
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
                    points[i, j] = (x * xAxis + y * yAxis + z * zAxis + coordinates).ToCartesianPoint(model);
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

        public static IfcCartesianPoint[] CreateSpiral(IModel model, double radius, double height, int numSegments, int numTurns, XbimVector3D displacement, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            xAxis = xAxis.Normalized();
            yAxis = yAxis.Normalized();
            XbimVector3D zAxis = XbimVector3D.CrossProduct(xAxis, yAxis).Normalized();
            
            double pitch = height / numTurns;
            
            IfcCartesianPoint[] points = new IfcCartesianPoint[numTurns * numSegments];
            for (int i = 0; i < numTurns * numSegments; i++)
            {
                double factor = i / (double)numSegments;
                double angle = 2 * Math.PI * factor;
                
                XbimVector3D x = xAxis * radius * Math.Cos(angle);
                XbimVector3D y = yAxis * radius * Math.Sin(angle);
                XbimVector3D z = zAxis * pitch * factor;
                
                XbimVector3D point = x + y + z + displacement;
                points[i] = IfcAxis.CreatePoint(model, point);
            }

            return points;
        }
        
        public static IfcCartesianPoint[] CreateSpiral(IModel model, double radius, double height, int numSegments, int numTurns, XbimVector3D displacement)
        {
            return CreateSpiral(model, radius, height, numSegments, numTurns, displacement, VectorExtensions.X, VectorExtensions.Y);
        }

        public static IfcCartesianPoint[,] CreateTorusPoints(IModel model, double torusRadius, double circleRadius, double angle, int numSegments, IfcAxisSettings axisSettings)
        {
            double angleStep = 2 * Math.PI / numSegments;
            double BendAngleStep = angle / (numSegments - 1);

            IfcCartesianPoint[,] ifcCartesianPoints = new IfcCartesianPoint[numSegments, numSegments];
            for (int i = 0; i < numSegments; i++)
            {
                for (int j = 0; j < numSegments; j++)
                {
                    double x = (torusRadius + circleRadius * Math.Cos(j * angleStep)) * Math.Cos(i * BendAngleStep);
                    double y = circleRadius * Math.Sin(j * angleStep);
                    double z = (torusRadius + circleRadius * Math.Cos(j * angleStep)) * Math.Sin(i * BendAngleStep);
                    ifcCartesianPoints[i, j] = IfcAxis.CreatePoint(model, new XbimVector3D(x, y, z));
                }
            }

            return ifcCartesianPoints;
        }
    }
}