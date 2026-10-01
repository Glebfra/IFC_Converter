using System.Collections.Generic;
using System.Linq;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh
{
    public sealed class Mesh : IMesh
    {
        private readonly List<FixedVector<Dim3>> _vertices;
        private readonly List<Triangle> _triangles;
        private readonly List<FixedVector<Dim3>> _normals;

        public IReadOnlyList<FixedVector<Dim3>> Vertices => _vertices;
        public IReadOnlyList<Triangle> Triangles => _triangles;
        public IReadOnlyList<FixedVector<Dim3>> Normals => _normals;
        
        public int VertexCount => _vertices.Count;
        public int TriangleCount => _triangles.Count;
        public int NormalCount => _normals.Count;
        
        public Mesh()
        {
            _vertices = new List<FixedVector<Dim3>>();
            _triangles = new List<Triangle>();
            _normals = new List<FixedVector<Dim3>>();
        }

        public Mesh(IEnumerable<FixedVector<Dim3>> vertices, IEnumerable<Triangle> triangles, IEnumerable<FixedVector<Dim3>> normals)
        {
            _vertices = vertices.ToList();
            _triangles = triangles.ToList();
            _normals = normals.ToList();
        }

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

        public int AddNormal(FixedVector<Dim3> normal)
        {
            _normals.Add(normal);
            return _normals.Count - 1;
        }
    }
}