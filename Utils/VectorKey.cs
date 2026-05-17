using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Utils
{
    public readonly struct VectorKey : IEquatable<VectorKey>
    {
        public Vector<double> Coordinates => new DenseVector(new double[]
        {
            _x * TOLERANCE,
            _y * TOLERANCE,
            _z * TOLERANCE,
        });

        private const double TOLERANCE = 1e-3;

        private readonly long _x;
        private readonly long _y;
        private readonly long _z;
        
        public VectorKey(Vector<double> point)
        {
            _x = Quantize(point[0]);
            _y = Quantize(point[1]);
            _z = Quantize(point[2]);
        }
        
        private static long Quantize(double value)
        {
            return (long)Math.Round(value / TOLERANCE);
        }

        public bool Equals(VectorKey other)
        {
            return _x == other._x &&
                   _y == other._y &&
                   _z == other._z;
        }

        public override bool Equals(object? obj)
        {
            return obj is VectorKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + _x.GetHashCode();
                hash = hash * 23 + _y.GetHashCode();
                hash = hash * 23 + _z.GetHashCode();

                return hash;
            }
        }
    }
}