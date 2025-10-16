using System;
using System.Collections.Generic;
using System.Linq;
using IFC.PropertySets;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Extensions
{
    public static class IfcTriangulatedFaceSetExtensions
    {
        public static int[][] GetIndices(this IfcTriangulatedFaceSet faceSet)
        {
            int len1 = faceSet.CoordIndex.Count;
            int[][] indices = new int[len1][];
            for (int i = 0; i < len1; i++)
            {
                IItemSet<IfcPositiveInteger> coordIndex = faceSet.CoordIndex[i];
                int len2 = coordIndex.Count;
                indices[i] = new int[len2];
                for (int j = 0; j < len2; j++)
                {
                    indices[i][j] = (int)coordIndex[j];
                }
            }

            return indices;
        }
        
        public static BendProperties GetBendProperties(this IfcTriangulatedFaceSet faceSet, AVEVA_Pset avevaPset)
        {
            XbimMatrix3D objectToWorldMatrix3D = avevaPset.GetObjectMatrix();
            XbimMatrix3D worldToObjectMatrix3D = objectToWorldMatrix3D.Inverted();
            XbimVector3D globalCoordinates = objectToWorldMatrix3D.Translation;
            XbimVector3D localCoordinates = worldToObjectMatrix3D.Transform(globalCoordinates);

            int[][] indices = GetIndices(faceSet);
            
            XbimVector3D[] globalVertices = faceSet.Coordinates.GetCoordinates().ToArray();
            XbimVector3D[] localVertices = globalVertices.Select(globalVertex => worldToObjectMatrix3D.Transform(globalVertex)).ToArray();

            XbimVector3D minLocalPoint = new XbimVector3D(
                localVertices.Min(vertex => vertex.X),
                localVertices.Min(vertex => vertex.Y),
                localVertices.Min(vertex => vertex.Z)
            );

            XbimVector3D[] firstPlaneLocalPoints = new XbimVector3D[3];
            XbimVector3D[] secondPlaneLocalPoints = new XbimVector3D[3];
            for (int i = 0; i < 3; i++)
            {
                firstPlaneLocalPoints[i] = localVertices[indices[indices.Length - 1][i] - 1];
                secondPlaneLocalPoints[i] = localVertices[indices[0][i] - 1];
            }

            XbimVector3D[] firstPlaneLocalVectors = new XbimVector3D[]
            {
                firstPlaneLocalPoints[0] - firstPlaneLocalPoints[1],
                firstPlaneLocalPoints[2] - firstPlaneLocalPoints[1]
            };
            XbimVector3D[] secondPlaneLocalVectors = new XbimVector3D[]
            {
                secondPlaneLocalPoints[0] - secondPlaneLocalPoints[1],
                secondPlaneLocalPoints[2] - secondPlaneLocalPoints[1]
            };

            XbimVector3D firstPlaneNorm = XbimVector3D.CrossProduct(firstPlaneLocalVectors[0], firstPlaneLocalVectors[1]).Normalized();
            XbimVector3D secondPlaneNorm = XbimVector3D.CrossProduct(secondPlaneLocalVectors[0], secondPlaneLocalVectors[1]).Normalized();

            double halfBendLength = minLocalPoint.X - localCoordinates.X;
            XbimVector3D firstPlaneCenter = localCoordinates + firstPlaneNorm * halfBendLength;
            XbimVector3D secondPlaneCenter = localCoordinates + secondPlaneNorm * halfBendLength;

            XbimVector3D[] boundPoints = new XbimVector3D[]
            {
                objectToWorldMatrix3D.Transform(firstPlaneCenter), 
                objectToWorldMatrix3D.Transform(secondPlaneCenter)
            };

            double firstPipeRadius = (firstPlaneCenter - firstPlaneLocalPoints[0]).Length * 2;
            double secondPipeRadius = (secondPlaneCenter - secondPlaneLocalPoints[0]).Length * 2;
            double pipeDiameter = Math.Max(firstPipeRadius, secondPipeRadius);

            double angle = firstPlaneNorm.Angle(secondPlaneNorm);

            return new BendProperties()
            {
                BoundPoints = boundPoints,
                Center = globalCoordinates,
                Angle = angle,
                PipeDiameter = pipeDiameter
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