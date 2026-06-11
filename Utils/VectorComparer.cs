using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;

namespace Utils
{
    public sealed class VectorComparer : IEqualityComparer<Vector<double>>
    {
        private readonly double _tolerance;

        public VectorComparer(double tolerance)
        {
            _tolerance = tolerance;
        }

        [Pure]
        public bool Equals(
            Vector<double>? x,
            Vector<double>? y)
        {
            if (x == null || y == null)
                return false;

            return
                Math.Abs(x[0] - y[0]) <= _tolerance &&
                Math.Abs(x[1] - y[1]) <= _tolerance &&
                Math.Abs(x[2] - y[2]) <= _tolerance;
        }

        [Pure]
        public int GetHashCode(Vector<double> obj)
        {
            long x = Quantize(obj[0]);
            long y = Quantize(obj[1]);
            long z = Quantize(obj[2]);

            unchecked
            {
                int hash = 17;

                hash = hash * 23 + x.GetHashCode();
                hash = hash * 23 + y.GetHashCode();
                hash = hash * 23 + z.GetHashCode();

                return hash;
            }
        }

        [Pure]
        public bool LessThan(
            Vector<double>? x,
            Vector<double>? y)
        {
            if (x == null || y == null)
                return false;

            return x.L2Norm() < y.L2Norm();
        }

        [Pure]
        public bool GreaterThan(
            Vector<double>? x,
            Vector<double>? y)
        {
            if (x == null || y == null)
                return false;

            return x.L2Norm() > y.L2Norm();
        }

        [Pure]
        public bool NearerThan(Vector<double>? x, Vector<double>? y, Vector<double> origin)
        {
            if (x == null || y == null)
                return false;

            return (x - origin).L2Norm() <= (y - origin).L2Norm();
        }

        [Pure]
        public bool FartherThan(Vector<double>? x, Vector<double>? y, Vector<double> origin)
        {
            if (x == null || y == null)
                return false;

            return (x - origin).L2Norm() >= (y - origin).L2Norm();
        }
        
        [Pure]
        public bool IsParallel(Vector<double> first, Vector<double> second)
        {
            return first.Normalize(2).CrossProduct(second.Normalize(2)).L2Norm() < _tolerance;
        }

        [Pure]
        private long Quantize(double value)
        {
            return (long)Math.Round(value / _tolerance);
        }
    }
}