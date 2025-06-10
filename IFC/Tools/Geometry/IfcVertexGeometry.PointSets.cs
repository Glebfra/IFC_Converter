using System;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;

namespace IFC.Tools.Geometry
{
    public static partial class IfcVertexGeometry
    {
        public static IfcCartesianPointList3D CreateCircle(IModel model, double radius, IfcAxisSettings axisSettings, int numSegments)
        {
            IfcCartesianPointList3D pointList3D = model.Instances.New<IfcCartesianPointList3D>();
            XbimVector3D[] points = CreateCirclePoints(model, radius, axisSettings, numSegments);
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
    }
}