using System;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Tools
{
    public static partial class IfcVertexGeometry
    {
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