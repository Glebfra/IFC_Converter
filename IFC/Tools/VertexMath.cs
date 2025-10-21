using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Extensions;
using Xbim.Common.Geometry;

namespace IFC.Tools
{
    public struct Triangle
    {
        public XbimVector3D[] Vertices;
        public int[] Indices;

        public static Triangle[] CreateFromVerticesAndIndices(XbimVector3D[] vertices, int[][] indices)
        {
            List<Triangle> triangles = new List<Triangle>();
            foreach (int[] triangleIndices in indices)
            {
                int i = 0;
                XbimVector3D[] triangleVertices = new XbimVector3D[3];
                foreach (int triangleIndex in triangleIndices)
                {
                    triangleVertices[i++] = vertices[triangleIndex - 1];
                }
                triangles.Add(new Triangle() {Vertices = triangleVertices, Indices = triangleIndices});
            }

            return triangles.ToArray();
        }
    }

    public struct Plane
    {
        public List<XbimVector3D> Points;
        public readonly XbimVector3D[] PlaneVectors;
        public readonly XbimVector3D NormalVector;
        
        public XbimVector3D Center => _center ??= GetCenter();
        private XbimVector3D? _center;

        private Plane(IEnumerable<XbimVector3D> points, XbimVector3D[] planeVectors)
        {
            Points = points.ToList();
            PlaneVectors = planeVectors;
            NormalVector = XbimVector3D.CrossProduct(PlaneVectors[0], PlaneVectors[1]);
        }

        public static Plane CreateFromTriangle(Triangle triangle)
        {
            XbimVector3D[] vertices = triangle.Vertices;
            XbimVector3D[] planeVectors =
            {
                vertices[0] - vertices[1],
                vertices[2] - vertices[1]
            };
            return new Plane(vertices, planeVectors);
        }

        public bool IsContainPoint(XbimVector3D point, double tolerance = 1e-6)
        {
            if (Points[0].IsEqualFixed(point, tolerance))
                return true;
            XbimVector3D pointVector = (point - Points[0]).Normalized();
            return Math.Abs(pointVector.DotProduct(NormalVector)) <= tolerance;
        }

        public bool IsCircle()
        {
            if (Points.Count <= 4)
                return false;
            double radius = (Points[0] - Center).Length;
            double tolerance = radius * 0.1;
            foreach (XbimVector3D point in Points)
                if (Math.Abs(radius - (point - Center).Length) > tolerance)
                    return false;
            return true;
        }

        public double GetCircleRadius()
        {
            if (!IsCircle())
                throw new Exception("The plane is not circle");
            return (Points[0] - Center).Length;
        }

        public bool IsEqual(Plane other)
        {
            return Center.IsEqualFixed(other.Center) && NormalVector.IsParallel(other.NormalVector);
        }

        private XbimVector3D GetCenter()
        {
            XbimVector3D minPoint = new XbimVector3D(
                Points.Min(point => point.X),
                Points.Min(point => point.Y),
                Points.Min(point => point.Z)
            );
            XbimVector3D maxPoint = new XbimVector3D(
                Points.Max(point => point.X),
                Points.Max(point => point.Y),
                Points.Max(point => point.Z)
            );

            return 0.5 * (minPoint + maxPoint);
        }
    }
}