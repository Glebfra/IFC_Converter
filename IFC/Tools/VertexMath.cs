using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Extensions;
using Xbim.Common.Geometry;

namespace IFC.Tools
{
    public struct Line
    {
        public double k;
        public double b;
        
        public double y(double x) => k * x + b;

        public Line CreatePerpendicularLine(XbimVector3D biasPoint)
        {
            double perpK = -1 / k;
            double perpB = biasPoint.Y - perpK * biasPoint.X;
            return new Line() { k = perpK, b = perpB };
        }

        public static Line CreateFromPoints(params XbimVector3D[] points)
        {
            double k = (points[1].Y - points[0].Y) / (points[1].X - points[0].X);
            double b = points[0].Y - k * points[0].X;
            return new Line() { k = k, b = b };
        }

        public static bool TryToCreateFromPoints(out Line line, params XbimVector3D[] points)
        {
            line = CreateFromPoints(points);
            return !double.IsInfinity(line.b) && !double.IsNaN(line.b) &&
                   !double.IsInfinity(line.k) && !double.IsNaN(line.k) &&
                   line.k is > 1e-3 and < 1e3;
        }

        public static Line CreatePerpendicularLine(Line line, XbimVector3D biasPoint)
        {
            return line.CreatePerpendicularLine(biasPoint);
        }

        public static XbimVector3D GetIntersectPoint(params Line[] lines) => GetIntersectPoint(lines[0], lines[1]);
        
        public static XbimVector3D GetIntersectPoint(Line first, Line second)
        {
            double x = -(second.b - first.b) / (second.k - first.k);
            double y = first.y(x);
            return new XbimVector3D(x, y, 0);
        }
    }
    
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
            NormalVector = XbimVector3D.CrossProduct(PlaneVectors[0], PlaneVectors[1]).Normalized();
        }

        public static Plane CreateFromTriangle(Triangle triangle)
        {
            XbimVector3D[] vertices = triangle.Vertices;
            XbimVector3D[] planeVectors =
            {
                (vertices[0] - vertices[1]).Normalized(),
                (vertices[2] - vertices[1]).Normalized()
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

        public bool IsEqual(Plane other, double precision=1e-3)
        {
            return Center.IsEqualFixed(other.Center, precision) && NormalVector.IsParallel(other.NormalVector);
        }
        
        private XbimVector3D GetCenter()
        {
            XbimVector3D globalPosition = Points[1];
            XbimVector3D xAxis = PlaneVectors[0];
            XbimVector3D zAxis = NormalVector;
            XbimVector3D yAxis = XbimVector3D.CrossProduct(zAxis, xAxis);
            XbimMatrix3D objectToWorld = XbimMatrix3D.CreateWorld(globalPosition, zAxis, yAxis);
            XbimMatrix3D worldToObject = objectToWorld.Inverted();

            XbimVector3D[] localPlanePoints = Points.Select(point => worldToObject.Transform(point)).ToArray();
            double z = localPlanePoints[0].Z;
            
            Line[] hordes = new Line[2];
            XbimVector3D[][] hordePoints = new XbimVector3D[2][];
            int index = 0;
            for (int i = 0; i < localPlanePoints.Length; i++)
            {
                for (int j = i + 1; j < localPlanePoints.Length; j++)
                {
                    XbimVector3D point1 = localPlanePoints[i], point2 = localPlanePoints[j];
                    XbimVector3D[] points = new XbimVector3D[] { point1, point2 };
                    if (!Line.TryToCreateFromPoints(out Line line, points)) 
                        continue;
                    hordes[index] = line;
                    hordePoints[index] = points;
                    index++;
                    if (index > 1)
                        goto cont;
                }
            }

            throw new Exception("Cannot find hordes in circle");
            
            cont:
            XbimVector3D[] centerPoints = hordePoints.Select(points => points.Average()).ToArray();
            Line[] perpHordes = hordes.Select((horde, i) => horde.CreatePerpendicularLine(centerPoints[i])).ToArray();
            XbimVector3D localCenterPoint = Line.GetIntersectPoint(perpHordes);
            localCenterPoint = new XbimVector3D(localCenterPoint.X, localCenterPoint.Y, z);
            XbimVector3D centerPoint = objectToWorld.Transform(localCenterPoint);
            
            return centerPoint;
        }
    }
}