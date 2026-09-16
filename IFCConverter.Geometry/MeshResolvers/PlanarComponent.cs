using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Geometry.MeshResolvers
{
    internal sealed class PlanarComponent
    {
        public IReadOnlyList<int> TriangleIndices { get; }
        public Vector<double> Normal { get; }
        public double Area { get; }
        
        public PlanarComponent(IReadOnlyList<int> triangleIndices, Vector<double> normal, double area)
        {
            TriangleIndices = triangleIndices;
            Normal = normal;
            Area = area;
        }
    }
}