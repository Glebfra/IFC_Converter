using System;
using System.Linq;
using IFC.PropertySets;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;

namespace IFC.Extensions
{
    public static class IfcTriangulatedFaceSetExtensions
    {
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

        public static ReducerProperties GetReducerProperties(this IfcTriangulatedFaceSet faceSet, AVEVA_Pset avevaPset, double tolerance = 1e-6)
        {
            XbimMatrix3D objectToWorldMatrix3D = avevaPset.GetObjectMatrix();
            XbimMatrix3D worldToObjectMatrix3D = objectToWorldMatrix3D.Inverted();
            XbimVector3D globalCoordinates = objectToWorldMatrix3D.Translation;
            
            XbimVector3D[] globalVertices = faceSet.Coordinates.GetCoordinates().ToArray();
            XbimVector3D[] localVertices = globalVertices.Select(globalVertex => worldToObjectMatrix3D.Transform(globalVertex)).ToArray();

            XbimVector3D localMinPoint = new XbimVector3D(
                localVertices.Min(vertex => vertex.X),
                localVertices.Min(vertex => vertex.Y),
                localVertices.Min(vertex => vertex.Z)
            );
            XbimVector3D localMaxPoint = new XbimVector3D(
                localVertices.Max(vertex => vertex.X),
                localVertices.Max(vertex => vertex.Y),
                localVertices.Max(vertex => vertex.Z)
            );

            XbimVector3D[] firstCircleLocalPoints = localVertices.Where(vertex => Math.Abs(vertex.X - localMinPoint.X) < tolerance).ToArray();
            XbimVector3D[] secondCircleLocalPoints = localVertices.Where(vertex => Math.Abs(vertex.X - localMaxPoint.X) < tolerance).ToArray();

            XbimVector3D firstCircleLocalCenterPoint = firstCircleLocalPoints.Average();
            XbimVector3D secondCircleLocalCenterPoint = secondCircleLocalPoints.Average();

            XbimVector3D centerDisplacement = secondCircleLocalCenterPoint - firstCircleLocalCenterPoint;
            XbimVector3D axisDisplacement = centerDisplacement.DotProduct(objectToWorldMatrix3D.Forward) * objectToWorldMatrix3D.Forward;

            XbimVector3D[] boundPoints = new XbimVector3D[]
            {
                objectToWorldMatrix3D.Transform(firstCircleLocalCenterPoint),
                objectToWorldMatrix3D.Transform(secondCircleLocalCenterPoint)
            };

            double[] radiuses = new double[]
            {
                (firstCircleLocalCenterPoint - firstCircleLocalPoints[0]).Length,
                (secondCircleLocalCenterPoint - secondCircleLocalPoints[0]).Length,
            };
            double length = (boundPoints[1] - boundPoints[0]).Length;

            return new ReducerProperties()
            {
                BoundPoints = boundPoints,
                Center = globalCoordinates,
                AxisDisplacement = axisDisplacement,
                ObjectMatrix3D = objectToWorldMatrix3D,
                Radiuses = radiuses,
                Length = length
            };
        }
    }
}