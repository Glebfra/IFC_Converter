using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Tools
{
    public static class IfcVertexPrimitives
    {
        public static IfcFacetedBrep CreateCone(IModel model, double radius, double height, XbimVector3D coordinates, int numSegments, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            xAxis = xAxis.Normalized();
            yAxis = yAxis.Normalized();
            XbimVector3D zAxis = XbimVector3D.CrossProduct(xAxis, yAxis).Normalized();
            
            IfcCartesianPoint[] botCircle = IfcVertexGeometry.CreateCircle(model, radius, coordinates, numSegments, xAxis, yAxis);
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
    }
}