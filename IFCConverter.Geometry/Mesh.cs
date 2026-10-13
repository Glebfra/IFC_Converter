using System.Collections.Generic;
using System.Linq;
using IFCConverter.Geometry.Algorithms;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry
{
    public readonly struct Mesh : IMesh
    {
        private readonly List<FixedVector<Dim3>> _vertices;
        private readonly List<Triangle> _triangles;
        private readonly List<FixedVector<Dim3>> _normals;

        public IReadOnlyList<FixedVector<Dim3>> Vertices => _vertices;
        public IReadOnlyList<Triangle> Triangles => _triangles;
        public IReadOnlyList<FixedVector<Dim3>> FaceNormals => _normals;
        
        public int VertexCount => _vertices.Count;
        public int TriangleCount => _triangles.Count;
        public int FaceNormalCount => _normals.Count;

        public Mesh(IEnumerable<FixedVector<Dim3>> vertices, IEnumerable<Triangle> triangles, IEnumerable<FixedVector<Dim3>> normals = null)
        {
            List<FixedVector<Dim3>> verticesList = vertices.ToList();
            _vertices = verticesList;
            _triangles = triangles.ToList();
            
            _normals = normals?.ToList() ?? _triangles
                .Select(triangle => MeshGeometry.GetFaceNormal(triangle.Select(triangleIndex => verticesList[triangleIndex]).ToArray()))
                .ToList();
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

        public int AddFaceNormal(FixedVector<Dim3> faceNormal)
        {
            _normals.Add(faceNormal);
            return _normals.Count - 1;
        }
    }
}