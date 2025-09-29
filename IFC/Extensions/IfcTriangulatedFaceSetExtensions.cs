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
    }
}