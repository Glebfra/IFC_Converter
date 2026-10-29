using System;
using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh.Builders
{
    public interface IMeshBuilder
    {
        List<FixedVector<Dim3>> Vertices { get; }
        List<Triangle> Triangles { get; }

        int VertexCount { get; }
        int TriangleCount { get; }
        int ChildCount { get; }

        IMeshBuilder AddPolygon(params int[] vertexIndices);

        IMeshBuilder Add(IMesh mesh);
        IMeshBuilder Add(IMeshBuilder builder);

        IMeshBuilder Translate(FixedVector<Dim3> offset);
        IMeshBuilder Transform(FixedMatrix<Dim3> rotate);
        IMeshBuilder Transform(FixedMatrix<Dim4> transform);
        IMeshBuilder Transform(Func<FixedVector<Dim3>, FixedVector<Dim3>> transform);

        IMeshBuilder ForEachVertex(Func<FixedVector<Dim3>, FixedVector<Dim3>> action);
        IMeshBuilder ForEachTriangle(Func<Triangle, Triangle> action);

        IMesh Build();
    }
}