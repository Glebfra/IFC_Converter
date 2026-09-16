using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.MeshResolvers
{
    internal sealed class PlanarComponent
    {

        public PlanarComponent(IReadOnlyList<int> triangleIndices, FixedVector<Dim3> normal, double area)
        {
            TriangleIndices = triangleIndices;
            Normal = normal;
            Area = area;
        }

        public IReadOnlyList<int> TriangleIndices { get; }
        public FixedVector<Dim3> Normal { get; }
        public double Area { get; }
    }
}