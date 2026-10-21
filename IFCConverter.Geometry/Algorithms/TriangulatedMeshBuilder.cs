using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Geometry.Mesh;
using IFCConverter.Geometry.Triangulators;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Algorithms
{
    public struct PlaneTriangulatedGeometryProperties
    {
        public FixedVector<Dim3>[] PolygonPoints;
    }
    
    public sealed class TriangulatedMeshBuilder : MeshBuilder
    {
        private const int NumSegments = 16;

        private readonly List<FixedVector<Dim3>> _vertices = new List<FixedVector<Dim3>>();
        private readonly List<Triangle> _triangles = new List<Triangle>();

        public int AddVertex(FixedVector<Dim3> vertex)
        {
            _vertices.Add(vertex);
            return _vertices.Count - 1;
        }

        public void AddTriangle(Triangle triangle)
        {
            _triangles.Add(triangle);
        }

        public void AddTriangle(int a, int b, int c)
        {
            _triangles.Add(new Triangle(a, b, c));
        }

        public void AddRectangle(int a, int b, int c, int d)
        {
            AddTriangle(a, b, c);
            AddTriangle(b, d, c);
        }
        
        [Pure]
        public override IMesh Build()
        {
            FixedVector<Dim3>[] vertices = _vertices.ToArray();
            Triangle[] triangles = _triangles.ToArray();

            return BuildMesh(vertices, triangles);
        }

        [Pure]
        public static IMesh CreatePolygon(PlaneTriangulatedGeometryProperties properties)
        {
            TriangulatedMeshBuilder builder = new TriangulatedMeshBuilder();
            foreach (FixedVector<Dim3> point in properties.PolygonPoints)
                builder.AddVertex(point);

            ITriangulator triangulator = new EarClippingTriangulator();
            Triangle[] triangles = triangulator.Triangulate(properties.PolygonPoints);
            
            foreach (Triangle triangle in triangles)
                builder.AddTriangle(triangle);

            return builder.Build();
        }
    }
}