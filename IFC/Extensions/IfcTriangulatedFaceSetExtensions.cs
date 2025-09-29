using System;
using System.Linq;
using IFC.PropertySets;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;

namespace IFC.Extensions
{
    public struct BendProperties
    {
        public XbimVector3D[] BoundPoints;
        public XbimVector3D Center;
        public double Radius;
        public double Angle;
    }

    public struct ReducerProperties
    {
        public XbimVector3D[] BoundPoints;
        public XbimVector3D Center;
        public double[] Radiuses;
        public double Length;
    }

    public static class IfcTriangulatedFaceSetExtensions
    {
        private const double TOLERANCE = 1e-6;
        
        public static BendProperties GetBendProperties(this IfcTriangulatedFaceSet faceSet, AVEVA_Pset avevaPset)
        {
            XbimMatrix3D objectMatrix3D = avevaPset.GetObjectMatrix();
            XbimVector3D coordinates = objectMatrix3D.Translation;
            XbimVector3D[] vertices = faceSet.Coordinates.GetCoordinates().ToArray();

            XbimVector3D minPoint = new XbimVector3D(
                vertices.Min(vertex => vertex.X),
                vertices.Min(vertex => vertex.Y),
                vertices.Min(vertex => vertex.Z)
            );
            
            XbimVector3D maxPoint = new XbimVector3D(
                vertices.Max(vertex => vertex.X),
                vertices.Max(vertex => vertex.Y),
                vertices.Max(vertex => vertex.Z)
            );

            XbimVector3D[] boundPoints = new XbimVector3D[]
            {
                coordinates - objectMatrix3D.Right.DotProduct(coordinates - minPoint) * objectMatrix3D.Right,
                coordinates + objectMatrix3D.Up.DotProduct(maxPoint - coordinates) * objectMatrix3D.Up,
            };

            XbimVector3D center = coordinates -
                                  objectMatrix3D.Right.DotProduct(coordinates - minPoint) * objectMatrix3D.Right +
                                  objectMatrix3D.Up.DotProduct(maxPoint - coordinates) * objectMatrix3D.Up;

            XbimVector3D[] displacementVectors = boundPoints.Select(boundPoint => boundPoint - center).ToArray();
            double radius = displacementVectors[0].Length;
            double angle = displacementVectors[0].Angle(displacementVectors[1]);
            
            return new BendProperties()
            {
                BoundPoints = boundPoints,
                Center = center,
                Radius = radius,
                Angle = angle
            };
        }

        public static ReducerProperties GetReducerProperties(this IfcTriangulatedFaceSet faceSet, AVEVA_Pset avevaPset)
        {
            XbimMatrix3D objectToWorldMatrix3D = avevaPset.GetObjectMatrix();
            XbimVector3D globalCoordinates = objectToWorldMatrix3D.Translation;
            XbimVector3D[] globalVertices = faceSet.Coordinates.GetCoordinates().ToArray();
            
            XbimVector3D globalMinPoint = new XbimVector3D(
                globalVertices.Min(vertex => vertex.X),
                globalVertices.Min(vertex => vertex.Y),
                globalVertices.Min(vertex => vertex.Z)
            );
            XbimVector3D globalMaxPoint = new XbimVector3D(
                globalVertices.Max(vertex => vertex.X),
                globalVertices.Max(vertex => vertex.Y),
                globalVertices.Max(vertex => vertex.Z)
            );
            
            XbimMatrix3D worldToObjectMatrix3D = objectToWorldMatrix3D.Inverted();
            XbimVector3D localMinPoint = worldToObjectMatrix3D.Transform(globalMinPoint);
            XbimVector3D localMaxPoint = worldToObjectMatrix3D.Transform(globalMaxPoint);
            XbimVector3D[] localVertices = globalVertices.Select(vertex => worldToObjectMatrix3D.Transform(vertex)).ToArray();

            XbimVector3D[] firstCircleLocalPoints = localVertices.Where(vertex => Math.Abs(vertex.X - localMinPoint.X) < TOLERANCE).ToArray();
            XbimVector3D[] secondCircleLocalPoints = localVertices.Where(vertex => Math.Abs(vertex.X - localMaxPoint.X) < TOLERANCE).ToArray();

            XbimVector3D secondCircleLocalCenterPoint = secondCircleLocalPoints.Average();

            XbimVector3D[] boundPoints = new XbimVector3D[]
            {
                globalCoordinates,
                objectToWorldMatrix3D.Transform(secondCircleLocalCenterPoint)
            };

            double[] radiuses = new double[]
            {
                (boundPoints[0] - firstCircleLocalPoints[0]).Length,
                (boundPoints[1] - secondCircleLocalPoints[0]).Length,
            };
            double length = (boundPoints[1] - boundPoints[0]).Length;

            return new ReducerProperties()
            {
                BoundPoints = boundPoints,
                Center = globalCoordinates,
                Radiuses = radiuses,
                Length = length
            };
        }
    }
}