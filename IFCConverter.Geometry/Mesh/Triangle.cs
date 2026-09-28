using System.Collections.Generic;

namespace IFCConverter.Geometry.Mesh
{
    public readonly struct Triangle
    {
        public int A { get; }
        public int B { get; }
        public int C { get; }

        public Triangle(int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Edge EdgeAB => new Edge(A, B);
        public Edge EdgeBC => new Edge(B, C);
        public Edge EdgeCA => new Edge(C, A);

        public IEnumerable<Edge> GetEdges()
        {
            yield return EdgeAB;
            yield return EdgeBC;
            yield return EdgeCA;
        }
    }
}