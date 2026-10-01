using System;
using System.Collections;
using System.Collections.Generic;

namespace IFCConverter.Geometry.Mesh
{
    public readonly struct Triangle : IEnumerable<int>
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

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return A;
                    case 1: return B;
                    case 2: return C;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public IEnumerable<Edge> GetEdges()
        {
            yield return EdgeAB;
            yield return EdgeBC;
            yield return EdgeCA;
        }

        public IEnumerator<int> GetEnumerator()
        {
            yield return A;
            yield return B;
            yield return C;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}