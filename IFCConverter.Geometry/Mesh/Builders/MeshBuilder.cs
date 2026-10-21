using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Geometry.Triangulators;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh.Builders
{
    public class MeshBuilder : IMeshBuilder
    {
        public List<FixedVector<Dim3>> Vertices { get; } = new List<FixedVector<Dim3>>();
        public List<Triangle> Triangles { get; } = new List<Triangle>();
        
        private readonly List<IMeshBuilder> _children = new List<IMeshBuilder>();
        private readonly List<Func<FixedVector<Dim3>, FixedVector<Dim3>>> _transforms = new List<Func<FixedVector<Dim3>, FixedVector<Dim3>>>();
        private readonly List<Func<FixedVector<Dim3>, FixedVector<Dim3>>> _forEachVertices = new List<Func<FixedVector<Dim3>, FixedVector<Dim3>>>();
        private readonly List<Func<Triangle, Triangle>> _forEachTriangles = new List<Func<Triangle, Triangle>>();

        public int VertexCount => Vertices.Count;
        public int TriangleCount => Triangles.Count;
        public int ChildCount => _children.Count;

        public int AddVertex(FixedVector<Dim3> vertex)
        {
            Vertices.Add(vertex);
            return Vertices.Count - 1;
        }

        public int AddTriangle(Triangle triangle)
        {
            Triangles.Add(triangle);
            return Triangles.Count - 1;
        }

        public void AddTriangles(IEnumerable<Triangle> triangles)
        {
            Triangles.AddRange(triangles);
        }

        public void AddRectangle(int a, int b, int c, int d)
        {
            AddTriangle(new Triangle(a, b, c));
            AddTriangle(new Triangle(b, d, c));
        }

        public IMeshBuilder AddPolygon(params int[] vertexIndices)
        {
            if (vertexIndices == null)
                throw new ArgumentNullException(nameof(vertexIndices));

            if (vertexIndices.Length < 3)
                throw new ArgumentException("Polygon must contain at least 3 vertices.", nameof(vertexIndices));

            FixedVector<Dim3>[] vertices = vertexIndices.Select(index => Vertices[index]).ToArray();
            Triangle[] triangles = new EarClippingTriangulator().Triangulate(vertices);

            foreach (Triangle triangle in triangles)
            {
                AddTriangle(new Triangle(
                    vertexIndices[triangle.A],
                    vertexIndices[triangle.B],
                    vertexIndices[triangle.C]
                ));
            }

            return this;
        }

        public IMeshBuilder Add(IMesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            int offset = Vertices.Count;
            Vertices.AddRange(mesh.Vertices);
            Triangles.AddRange(mesh.Triangles.Select(triangle => new Triangle(triangle.A + offset, triangle.B + offset, triangle.C + offset)));

            return this;
        }

        public IMeshBuilder Add(IMeshBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (ReferenceEquals(builder, this))
                throw new ArgumentException("A builder cannot contain itself.", nameof(builder));

            _children.Add(builder);
            return this;
        }

        public IMeshBuilder Translate(FixedVector<Dim3> offset)
        {
            return Transform(vertex => vertex + offset);
        }

        public IMeshBuilder Transform(FixedMatrix<Dim3> rotate)
        {
            return Transform(vertex => rotate * vertex);
        }

        public IMeshBuilder Transform(FixedMatrix<Dim4> transform)
        {
            return Transform(vertex => (transform * vertex.ToHomogeneous(1)).ToCartesian());
        }

        public IMeshBuilder Transform(Func<FixedVector<Dim3>, FixedVector<Dim3>> transform)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));
            
            _transforms.Add(transform);
            return this;
        }

        public IMeshBuilder ForEachVertex(Func<FixedVector<Dim3>, FixedVector<Dim3>> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
         
            _forEachVertices.Add(action);
            return this;
        }

        public IMeshBuilder ForEachTriangle(Func<Triangle, Triangle> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            
            _forEachTriangles.Add(action);
            return this;
        }

        [Pure]
        public virtual IMesh Build()
        {
            List<FixedVector<Dim3>> vertices = new List<FixedVector<Dim3>>(Vertices);
            List<Triangle> triangles = new List<Triangle>(Triangles);
            
            foreach (IMeshBuilder child in _children)
            {
                IMesh mesh = child.Build();
                int offset = vertices.Count;
                
                vertices.AddRange(mesh.Vertices);
                triangles.AddRange(mesh.Triangles.Select(triangle => new Triangle(triangle.A + offset, triangle.B + offset, triangle.C + offset)));
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                FixedVector<Dim3> vertex = vertices[i];
                foreach (Func<FixedVector<Dim3>, FixedVector<Dim3>> transform in _transforms)
                {
                    vertex = transform(vertex);
                }

                vertices[i] = vertex;
            }

            FixedVector<Dim3>[] vertexArr = vertices.ToArray();
            Triangle[] trianglesArr = triangles.ToArray();
            
            foreach (Func<FixedVector<Dim3>, FixedVector<Dim3>> forEachVertex in _forEachVertices)
            {
                for (int i = 0; i < vertexArr.Length; i++)
                {
                    vertexArr[i] = forEachVertex(vertexArr[i]);
                }
            }
            
            foreach (Func<Triangle, Triangle> forEachTriangle in _forEachTriangles)
            {
                for (int i = 0; i < trianglesArr.Length; i++)
                {
                    trianglesArr[i] = forEachTriangle(trianglesArr[i]);
                }
            }
            
            ValidateTriangles(vertexArr.Length, trianglesArr);
            return new Mesh(vertexArr, trianglesArr);
        }
        
        protected static void ValidatePositiveFinite(double value, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, "Value must be finite and greater than zero.");
            }
        }

        protected static void ValidateFinite(double value, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(parameterName, "Value must be finite.");
            }
        }
        
        private static void ValidateTriangles(int vertexCount, Triangle[] triangles)
        {
            foreach (Triangle triangle in triangles)
            {
                if (triangle.A < 0 || triangle.A >= vertexCount ||
                    triangle.B < 0 || triangle.B >= vertexCount ||
                    triangle.C < 0 || triangle.C >= vertexCount)
                {
                    throw new InvalidOperationException(
                        "A triangle references a vertex outside the mesh.");
                }
            }
        }
    }
}