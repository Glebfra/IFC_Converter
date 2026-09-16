using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace IFCConverter.Utils.Mathematics
{
    public sealed class VectorComparer : IEqualityComparer<FixedVector<Dim3>>
    {
        private readonly double _tolerance;

        public VectorComparer(double tolerance)
        {
            _tolerance = tolerance;
        }

        [Pure]
        public bool Equals(FixedVector<Dim3> x, FixedVector<Dim3> y)
        {
            if (x == null || y == null)
                return false;

            return
                Math.Abs(x[0] - y[0]) <= _tolerance &&
                Math.Abs(x[1] - y[1]) <= _tolerance &&
                Math.Abs(x[2] - y[2]) <= _tolerance;
        }

        [Pure]
        public int GetHashCode(FixedVector<Dim3> obj)
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
        public bool LessThan(FixedVector<Dim3> x, FixedVector<Dim3> y)
        {
            if (x == null || y == null)
                return false;

            return x.L2Norm() < y.L2Norm();
        }

        [Pure]
        public bool GreaterThan(FixedVector<Dim3> x, FixedVector<Dim3> y)
        {
            if (x == null || y == null)
                return false;

            return x.L2Norm() > y.L2Norm();
        }

        [Pure]
        public bool NearerThan(FixedVector<Dim3> x, FixedVector<Dim3> y, FixedVector<Dim3> origin)
        {
            if (x == null || y == null)
                return false;

            return (x - origin).L2Norm() <= (y - origin).L2Norm();
        }

        [Pure]
        public bool FartherThan(FixedVector<Dim3> x, FixedVector<Dim3> y, FixedVector<Dim3> origin)
        {
            if (x == null || y == null)
                return false;

            return (x - origin).L2Norm() >= (y - origin).L2Norm();
        }

        [Pure]
        public bool IsParallel(FixedVector<Dim3> first, FixedVector<Dim3> second)
        {
            return first.Normalize().CrossProduct(second.Normalize()).L2Norm() < _tolerance;
        }

        [Pure]
        private long Quantize(double value)
        {
            return (long)Math.Round(value / _tolerance);
        }
    }
}