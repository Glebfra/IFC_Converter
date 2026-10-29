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
            if (double.IsNaN(tolerance) || double.IsInfinity(tolerance) || tolerance <= 0)
                throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must be a positive number.");

            _tolerance = tolerance;
        }

        [Pure]
        public bool Equals(FixedVector<Dim3> x, FixedVector<Dim3> y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return Quantize(x[0]) == Quantize(y[0]) &&
                   Quantize(x[1]) == Quantize(y[1]) &&
                   Quantize(x[2]) == Quantize(y[2]);
        }

        [Pure]
        public int GetHashCode(FixedVector<Dim3> obj)
        {
            if (obj == null)
                return 0;

            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Quantize(obj[0]).GetHashCode();
                hash = hash * 23 + Quantize(obj[1]).GetHashCode();
                hash = hash * 23 + Quantize(obj[2]).GetHashCode();

                return hash;
            }
        }

        [Pure]
        private long Quantize(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be finite.");

            double quantized = Math.Floor(value / _tolerance);
            if (quantized < long.MinValue || quantized >= long.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value));

            return (long)quantized;
        }
    }
}