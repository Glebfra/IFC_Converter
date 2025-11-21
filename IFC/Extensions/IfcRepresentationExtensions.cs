using System;
using System.Collections.Generic;
using System.Linq;
using IFC.PropertySets;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Extensions
{
    public static class IfcRepresentationExtensions
    {
        public static IfcTriangulatedFaceSet CreateTriangulatedFaceSet(IModel model, IfcCartesianPointList3D cartesianPointList3D, int[][] indices, XbimVector3D[]? normals=null)
        {
            return model.Instances.New<IfcTriangulatedFaceSet>(set =>
            {
                set.Coordinates = cartesianPointList3D;

                for (int i = 0; i < indices.Length; i++)
                {
                    set.CoordIndex.GetAt(i).AddRange(indices[i].Select(index => new IfcPositiveInteger(index)));
                }

                if (normals != null)
                {
                    for (int i = 0; i < normals.Length; i++)
                    {
                        set.Normals.GetAt(i).AddRange(new IfcParameterValue[] { normals[i].X, normals[i].Y, normals[i].Z });
                    }
                }
            });
        }

        public static IfcTriangulatedFaceSet CreateTriangulatedFaceSet(IModel model, XbimVector3D[] vertices, int[][] indices, XbimVector3D[]? normals = null)
        {
            IfcCartesianPointList3D cartesianPointList3D = IfcAxisExtensions.CreateCartesianPointList3D(model, vertices);
            return CreateTriangulatedFaceSet(model, cartesianPointList3D, indices, normals);
        }

        public static FlangeProperties GetFlangeProperties(this IfcRepresentation ifcRepresentation, AVEVA_Pset avevaPset)
        {
            XbimVector3D center = avevaPset.GetPosition();
            
            IfcExtrudedAreaSolid? extrudedAreaSolid = ifcRepresentation.Items.OfType<IfcExtrudedAreaSolid>().FirstOrDefault();
            if (extrudedAreaSolid == null)
                throw new Exception($"Cannot find {nameof(IfcExtrudedAreaSolid)} in {nameof(IfcRepresentationItem)}");
            PipeProperties pipeProperties = extrudedAreaSolid.GetPipeProperties();

            IfcTriangulatedFaceSet? triangulatedFaceSet = ifcRepresentation.Items.OfType<IfcTriangulatedFaceSet>().FirstOrDefault();
            if (triangulatedFaceSet == null)
                throw new Exception($"Cannot find {nameof(IfcTriangulatedFaceSet)} in {nameof(IfcRepresentationItem)}");
            ReducerProperties reducerProperties = triangulatedFaceSet.GetReducerProperties(avevaPset);

            return new FlangeProperties()
            {
                Center = center,
                BoundPoints = new XbimVector3D[] { pipeProperties.BoundPoints[0], reducerProperties.BoundPoints[1] }
            };
        }

        public static PipeProperties GetPipeProperties(this IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            XbimVector3D[] boundPoints = GetBoundPoints(extrudedAreaSolid);
            XbimMatrix3D areaSolidMatrix3D = extrudedAreaSolid.Position.ToMatrix3D();
            XbimVector3D forward = areaSolidMatrix3D.Transform(extrudedAreaSolid.ExtrudedDirection.XbimVector3D());
            XbimVector3D globalFirstPoint = areaSolidMatrix3D.Translation;
            double radius = GetCircleRadius(extrudedAreaSolid);
            double length = extrudedAreaSolid.Depth;

            return new PipeProperties()
            {
                Radius = radius,
                BoundPoints = boundPoints,
                Direction = forward,
                Length = length,
                Coordinates = globalFirstPoint
            };
        }
        
        private static XbimVector3D[] GetBoundPoints(this IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            XbimMatrix3D areaSolidMatrix3D = extrudedAreaSolid.Position.ToMatrix3D();
            XbimVector3D forward = extrudedAreaSolid.ExtrudedDirection.XbimVector3D();
            double length = extrudedAreaSolid.Depth;
            
            XbimVector3D internalSecondPoint = forward * length;

            XbimVector3D globalFirstPoint = areaSolidMatrix3D.Translation;
            XbimVector3D globalSecondPoint = globalFirstPoint + areaSolidMatrix3D.Transform(internalSecondPoint);

            return new XbimVector3D[] { globalFirstPoint, globalSecondPoint };
        }
        
        private static double GetCircleRadius(this IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            if (extrudedAreaSolid.SweptArea is IfcCircleProfileDef circleProfileDef)
            {
                return circleProfileDef.Radius;
            }

            throw new ArgumentException($"{nameof(extrudedAreaSolid)} does not contain {nameof(IfcCircleProfileDef)}");
        }
        
        public static BendProperties GetBendProperties(this IfcRevolvedAreaSolid revolvedAreaSolid)
        {
            XbimVector3D internalAxisLocation = revolvedAreaSolid.Axis.Location.ToXbimVector3D();
            XbimVector3D internalAxisDirection = revolvedAreaSolid.Axis.Axis.XbimVector3D();

            double angle = revolvedAreaSolid.Angle;
            XbimVector3D internalFirstPoint = internalAxisLocation.Negated();
            // In XbimVector3D, rotation occurs along the left trio of vectors for some reason. Therefore, for correct calculations, we use the minus angle.
            XbimVector3D internalSecondPoint = internalFirstPoint.RotateAroundAxis(internalAxisDirection, -angle);

            XbimMatrix3D areaSolidMatrix3D = revolvedAreaSolid.Position.ToMatrix3D();
            XbimVector3D areaSolidDisplacement = areaSolidMatrix3D.Translation + areaSolidMatrix3D.Transform(internalAxisLocation);

            XbimVector3D globalFirstPoint = areaSolidMatrix3D.Transform(internalFirstPoint) + areaSolidDisplacement;
            XbimVector3D globalSecondPoint = areaSolidMatrix3D.Transform(internalSecondPoint) + areaSolidDisplacement;
            XbimVector3D globalAxisLocation = areaSolidMatrix3D.Translation + internalAxisLocation;
            
            XbimVector3D[] boundPoints = new XbimVector3D[] { globalFirstPoint, globalSecondPoint };

            double pipeDiameter = revolvedAreaSolid.SweptArea is IfcCircleProfileDef circleProfileDef ? circleProfileDef.Radius * 2 : 0;

            return new BendProperties()
            {
                Angle = angle,
                BoundPoints = boundPoints,
                Center = globalAxisLocation,
                Radius = internalAxisLocation.Length,
                PipeDiameter = pipeDiameter
            };
        }
        
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

        public static Triangle[] GetTriangles(this IfcTriangulatedFaceSet faceSet)
        {
            XbimVector3D[] vertices = faceSet.Coordinates.GetCoordinates().ToArray();
            int[][] indices = faceSet.GetIndices();

            return Triangle.CreateFromVerticesAndIndices(vertices, indices);
        }

        public static PipeProperties GetPipeProperties(this IfcTriangulatedFaceSet faceSet)
        {
            XbimVector3D[] vertices = faceSet.Coordinates.GetCoordinates().ToArray();
            Triangle[] triangles = faceSet.GetTriangles();

            IEnumerable<Plane> trianglePlanes = triangles.Select(Plane.CreateFromTriangle);
            IEnumerable<Plane> planes = Plane.UpdatePlanesByVertices(trianglePlanes, vertices);
            IEnumerable<Plane> circlePlanes = Plane.GetCirclePlanes(planes);
            Plane[] pipePlanes = Plane.GetPipePlanes(circlePlanes).ToArray();

            double radius = pipePlanes[0].GetCircleRadius();
            XbimVector3D[] boundPoints = pipePlanes.Select(pipePlane => pipePlane.Center).ToArray();
            XbimVector3D direction = boundPoints[1] - boundPoints[0];
            XbimVector3D coordinates = boundPoints[0];
            double length = direction.Length;
            direction = direction.Normalized();

            return new PipeProperties()
            {
                Coordinates = coordinates,
                Radius = radius,
                Length = length,
                Direction = direction,
                BoundPoints = boundPoints
            };
        }

        // TODO change to the new version of creation
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

            double angle = firstPlaneNorm.Angle(secondPlaneNorm.Negated());
            double radius = (firstPlaneCenter - secondPlaneCenter).Length / (2 * Math.Sin(angle / 2));

            return new BendProperties()
            {
                BoundPoints = boundPoints,
                Center = globalCoordinates,
                Angle = angle,
                PipeDiameter = pipeDiameter,
                Radius = radius
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