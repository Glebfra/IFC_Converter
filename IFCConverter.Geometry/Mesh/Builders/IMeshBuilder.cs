using System;
using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh.Builders
{
    public interface IMeshBuilder
    {
        int VertexCount { get; }
        int TriangleCount { get; }
        int ChildCount { get; }

        int AddVertex(FixedVector<Dim3> vertex);
        
        int AddTriangle(Triangle triangle);
        void AddTriangles(IEnumerable<Triangle> triangles);
        
        void AddRectangle(int a, int b, int c, int d);

        IMeshBuilder Add(IMesh mesh);
        IMeshBuilder Add(IMeshBuilder builder);
        
        IMeshBuilder Translate(FixedVector<Dim3> offset);
        IMeshBuilder Transform(FixedMatrix<Dim3> rotate);
        IMeshBuilder Transform(FixedMatrix<Dim4> transform);
        IMeshBuilder Transform(Func<FixedVector<Dim3>, FixedVector<Dim3>> transform);

        IMesh Build();
    }
}