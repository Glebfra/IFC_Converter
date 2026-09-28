using System;

namespace IFCConverter.Geometry.Mesh
{
    public readonly struct Edge : IEquatable<Edge>
    {
        public int A { get; }
        public int B { get; }

        public Edge(int a, int b)
        {
            if (a <= b)
            {
                A = a;
                B = b;
            }
            else
            {
                A = b;
                B = a;
            }
        }
        
        public bool Equals(Edge other)
        {
            return A == other.A && B == other.B;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Edge)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (A * 397) ^ B;
            }
        }

        public override string ToString()
        {
            return $"({A}, {B})";
        }
    }
}