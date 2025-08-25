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
            
            return CreateCone(model, botCircle, topPoint);
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

        public static IfcFacetedBrep CreateClippedCone(
            IModel model, double botRadius, double topRadius, double height, 
            XbimVector3D coordinates, int numSegments, 
            XbimVector3D xAxis, XbimVector3D yAxis
        )
        {
            xAxis = xAxis.Normalized();
            yAxis = yAxis.Normalized();
            XbimVector3D zAxis = XbimVector3D.CrossProduct(xAxis, yAxis).Normalized();
            XbimVector3D topCoordinates = coordinates + height * zAxis;
            
            IfcCartesianPoint[] botCircle = CreateCircle(model, botRadius, coordinates, numSegments, xAxis, yAxis);
            IfcCartesianPoint[] topCircle = CreateCircle(model, topRadius, topCoordinates, numSegments, xAxis, yAxis);

            return CreateClippedCone(model, botCircle, topCircle);
        }
        
        public static IfcFacetedBrep CreateClippedCone(
            IModel model, double botRadius, double topRadius, double height, int numSegments, 
            IfcAxisSettings axisSettings
        )
        {
            XbimVector3D coordinates = axisSettings.Origin;
            XbimVector3D xAxis = axisSettings.XAxis.Normalized();
            XbimVector3D yAxis = axisSettings.YAxis.Normalized();
            XbimVector3D zAxis = XbimVector3D.CrossProduct(xAxis, yAxis).Normalized();
            XbimVector3D topCoordinates = coordinates + height * zAxis;
            
            IfcCartesianPoint[] botCircle = CreateCircle(model, botRadius, coordinates, numSegments, xAxis, yAxis);
            IfcCartesianPoint[] topCircle = CreateCircle(model, topRadius, topCoordinates, numSegments, xAxis, yAxis);

            return CreateClippedCone(model, botCircle, topCircle);
        }

        public static IfcFacetedBrep CreateClippedCone(
            IModel model, double botRadius, double topRadius, double height, int numSegments, 
            IfcAxisSettings axisSettings, XbimVector3D topDisplacement
        )
        {
            XbimVector3D xAxis = axisSettings.XAxis.Normalized();
            XbimVector3D yAxis = axisSettings.YAxis.Normalized();
            XbimVector3D zAxis = axisSettings.ZAxis.Normalized();
            XbimVector3D botCoordinates = axisSettings.Origin;
            XbimVector3D topCoordinates = botCoordinates + topDisplacement + height * zAxis;

            IfcCartesianPoint[] botCircle = CreateCircle(model, botRadius, botCoordinates, numSegments, xAxis, yAxis);
            IfcCartesianPoint[] topCircle = CreateCircle(model, topRadius, topCoordinates, numSegments, xAxis, yAxis);

            return CreateClippedCone(model, botCircle, topCircle);
        }

        public static IfcFacetedBrep CreateCylinder(IModel model, double radius, double height, XbimVector3D coordinates, int numSegments, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            return CreateClippedCone(model, radius, radius, height, coordinates, numSegments, xAxis, yAxis);
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

        public static IfcFacetedBrep CreateSphere(IModel model, double radius, XbimVector3D coordinates, int numSegments, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            IfcCartesianPoint[,] spherePoints = CreateSpherePoints(model, radius, coordinates, numSegments, xAxis, yAxis);
            return CreateSphere(model, spherePoints);
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

        public static IfcFacetedBrep CreateTorus(IModel model, double torusRadius, double circleRadius, double angle, int numSegments, IfcAxisSettings axisSettings)
        {
            IfcCartesianPoint[,] points = CreateTorusPoints(model, torusRadius, circleRadius, angle, numSegments, axisSettings);
            return CreateTorus(model, points);
        }

        public static IfcFacetedBrep CreateTorus(IModel model, IfcCartesianPoint[,] points)
        {
            int length1 = points.GetLength(0);
            int length2 = points.GetLength(1);
            
            IfcFace[] faces = new IfcFace[(length1 - 1) * length2];
            int facesIndex = 0;
            for (int i = 0; i < length1 - 1; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    IfcCartesianPoint p1 = points[i, j];
                    IfcCartesianPoint p2 = points[i, (j + 1) % length2];
                    IfcCartesianPoint p3 = points[i + 1, (j + 1) % length2];
                    IfcCartesianPoint p4 = points[i + 1, j];
                    faces[facesIndex++] = CreateRectangleFace(model, p1, p2, p3, p4);
                }
            }

            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }
    }
}