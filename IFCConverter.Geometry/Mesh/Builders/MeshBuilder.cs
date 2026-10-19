using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh.Builders
{
    public class MeshBuilder : IMeshBuilder
    {
        private readonly List<FixedVector<Dim3>> _vertices = new List<FixedVector<Dim3>>();
        private readonly List<Triangle> _triangles = new List<Triangle>();
        
        private readonly List<IMeshBuilder> _children = new List<IMeshBuilder>();
        private readonly List<Func<FixedVector<Dim3>, FixedVector<Dim3>>> _transforms = new List<Func<FixedVector<Dim3>, FixedVector<Dim3>>>();

        public int VertexCount => _vertices.Count;
        public int TriangleCount => _triangles.Count;
        public int ChildCount => _children.Count;

        public int AddVertex(FixedVector<Dim3> vertex)
        {
            _vertices.Add(vertex);
            return _vertices.Count - 1;
        }

        public int AddTriangle(Triangle triangle)
        {
            _triangles.Add(triangle);
            return _triangles.Count - 1;
        }

        public void AddTriangles(IEnumerable<Triangle> triangles)
        {
            _triangles.AddRange(triangles);
        }

        public void AddRectangle(int a, int b, int c, int d)
        {
            AddTriangle(new Triangle(a, b, c));
            AddTriangle(new Triangle(b, d, c));
        }

        public IMeshBuilder Add(IMesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            int offset = _vertices.Count;
            _vertices.AddRange(mesh.Vertices);
            _triangles.AddRange(mesh.Triangles.Select(triangle => new Triangle(triangle.A + offset, triangle.B + offset, triangle.C + offset)));

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

        [Pure]
        public virtual IMesh Build()
        {
            List<FixedVector<Dim3>> vertices = new List<FixedVector<Dim3>>(_vertices);
            List<Triangle> triangles = new List<Triangle>(_triangles);
            
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
            
            ValidateTriangles(vertexArr.Length, trianglesArr);

            return new Mesh(vertexArr, trianglesArr);
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