using System.Collections.Generic;

namespace IFCConverter.Geometry
{
    public class PlanarRegion
    {
        public IReadOnlyList<int> TriangleIndices { get; }
        public IReadOnlyList<Edge> BoundaryEdges { get; }

        public double Area { get; }
        public double Perimeter { get; }
        public double MaxDeviation { get; }
        public double RmsDeviation { get; }
        public double Compactness { get; }

        public double Score { get; internal set; }
        
        public int TriangleCount => TriangleIndices.Count;

        public PlanarRegion(IReadOnlyList<int> triangleIndices, IReadOnlyList<Edge> boundaryEdges, double area, double perimeter, double maxDeviation, double rmsDeviation, double compactness)
        {
            TriangleIndices = triangleIndices;
            BoundaryEdges = boundaryEdges;
            Area = area;
            Perimeter = perimeter;
            MaxDeviation = maxDeviation;
            RmsDeviation = rmsDeviation;
            Compactness = compactness;
        }
    }
}