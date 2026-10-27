using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Utils.Mathematics
{
    public class FixedVectorBuilder<TDimension>
        where TDimension : struct, IDimension
    {
        private static int Dimension => Dimension<TDimension>.Dim;

        public FixedVector<TDimension> Dense()
        {
            return new FixedVector<TDimension>(Vector<double>.Build.Dense(Dimension));
        }

        public FixedVector<TDimension> Dense(double value)
        {
            return new FixedVector<TDimension>(Vector<double>.Build.Dense(Dimension, value));
        }

        public FixedVector<TDimension> Dense(params double[] values)
        {
            if (values.Length != Dimension)
                throw new ArgumentException($"The vector length {values.Length} is not equal to the dimension {Dimension}.");

            return new FixedVector<TDimension>(Vector<double>.Build.Dense(values));
        }

        public FixedVector<TDimension> Zeros()
        {
            return Dense(0.0);
        }

        public FixedVector<TDimension> Ones()
        {
            return Dense(1.0);
        }
    }

    public class FixedVector<TDimension> : IEnumerable<double>, IEquatable<FixedVector<TDimension>>
        where TDimension : struct, IDimension
    {

        public FixedVector(Vector<double> vector)
        {
            if (vector == null)
                throw new ArgumentNullException(nameof(vector));

            if (vector.Count != Dimension)
                throw new ArgumentException($"The vector dimension {vector.Count} is not equal to the dimension {Dimension}.");

            Vector = vector;
        }

        private static int Dimension => Dimension<TDimension>.Dim;
        public static FixedVectorBuilder<TDimension> Builder => new FixedVectorBuilder<TDimension>();

        internal Vector<double> Vector { get; }

        public double this[int index]
        {
            get => Vector[index];
            set => Vector[index] = value;
        }

        public IEnumerator<double> GetEnumerator()
        {
            for (int i = 0; i < Vector.Count; i++)
                yield return Vector[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(FixedVector<TDimension> other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Vector.Equals(other.Vector);
        }

        public double Dot(FixedVector<TDimension> other)
        {
            return Dot(this, other);
        }

        public double L2Norm()
        {
            return L2Norm(this);
        }

        public double L1Norm()
        {
            return L1Norm(this);
        }

        public bool IsParallel(FixedVector<TDimension> vector, double maximumAbsoluteError = 1e-6)
        {
            return IsParallel(this, vector, maximumAbsoluteError);
        }

        public bool IsNormal(FixedVector<TDimension> b, double maximumAbsoluteError = 1e-6)
        {
            return IsNormal(this, b, maximumAbsoluteError);
        }

        public FixedVector<TDimension> Normalize(double tolerance = 1e-6)
        {
            return Normalize(this, tolerance);
        }

        public FixedVector<TDimension> SafeNormalize(double tolerance = 1e-6)
        {
            return SafeNormalize(this, tolerance);
        }

        public FixedVector<TDimension> Negate()
        {
            return Negate(this);
        }

        public bool AlmostEqual(FixedVector<TDimension> other, double maximumAbsoluteError = 1e-6)
        {
            return AlmostEqual(this, other, maximumAbsoluteError);
        }

        public bool IsZero(double tolerance = 1e-6)
        {
            return IsZero(this, tolerance);
        }

        public bool IsFinite()
        {
            return IsFinite(this);
        }

        public bool IsLessThan(FixedVector<TDimension> other)
        {
            return IsLessThan(this, other);
        }

        public bool IsGreaterThan(FixedVector<TDimension> other)
        {
            return IsGreaterThan(this, other);
        }

        public bool IsNearerThan(FixedVector<TDimension> other, FixedVector<TDimension> origin)
        {
            return IsNearerThan(this, other, origin);
        }

        public bool IsFartherThan(FixedVector<TDimension> other, FixedVector<TDimension> origin)
        {
            return IsFartherThan(this, other, origin);
        }

        public double Angle(FixedVector<TDimension> other)
        {
            return Angle(this, other);
        }

        public double DistanceTo(FixedVector<TDimension> other)
        {
            return DistanceTo(this, other);
        }

        public double DistanceSquaredTo(FixedVector<TDimension> other)
        {
            return DistanceSquaredTo(this, other);
        }

        public override string ToString()
        {
            return Vector.ToString();
        }

        public static FixedVector<TDimension> operator +(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return Add(a, b);
        }

        public static FixedVector<TDimension> operator -(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return Subtract(a, b);
        }

        public static double operator *(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return Multiply(a, b);
        }

        public static FixedVector<TDimension> operator *(FixedVector<TDimension> a, double scalar)
        {
            return Multiply(a, scalar);
        }

        public static FixedVector<TDimension> operator *(double scalar, FixedVector<TDimension> a)
        {
            return Multiply(a, scalar);
        }

        public static FixedVector<TDimension> Zeros()
        {
            return new FixedVector<TDimension>(Vector<double>.Build.Dense(Dimension, 0.0));
        }

        public static FixedVector<TDimension> Ones()
        {
            return new FixedVector<TDimension>(Vector<double>.Build.Dense(Dimension, 1.0));
        }

        public static FixedVector<TDimension> Add(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return new FixedVector<TDimension>(a.Vector.Add(b.Vector));
        }

        public static FixedVector<TDimension> Subtract(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return new FixedVector<TDimension>(a.Vector.Subtract(b.Vector));
        }

        public static FixedVector<TDimension> Multiply(FixedVector<TDimension> a, double scalar)
        {
            return new FixedVector<TDimension>(a.Vector * scalar);
        }

        public static double Multiply(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return Dot(a, b);
        }

        public static double Dot(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return a.Vector.DotProduct(b.Vector);
        }

        public static double L2Norm(FixedVector<TDimension> vector)
        {
            return vector.Vector.L2Norm();
        }

        public static double L1Norm(FixedVector<TDimension> vector)
        {
            return vector.Vector.L1Norm();
        }

        public static bool IsParallel(FixedVector<TDimension> a, FixedVector<TDimension> b, double maximumAbsoluteError = 1e-6)
        {
            double aLength = a.L2Norm();
            double bLength = b.L2Norm();

            if (aLength <= maximumAbsoluteError ||
                bLength <= maximumAbsoluteError)
                return false;

            double dot = a.Dot(b) / (aLength * bLength);
            return Math.Abs(Math.Abs(dot) - 1.0) <= maximumAbsoluteError;
        }

        public static bool IsNormal(FixedVector<TDimension> a, FixedVector<TDimension> b, double maximumAbsoluteError = 1e-6)
        {
            return Math.Abs(a.Dot(b) / (a.L2Norm() * b.L2Norm())) < maximumAbsoluteError;
        }

        public static FixedVector<TDimension> Normalize(FixedVector<TDimension> vector, double tolerance = 1e-6)
        {
            if (vector.L2Norm() <= tolerance)
                throw new InvalidOperationException();
            
            return new FixedVector<TDimension>(vector.Vector.Normalize(2));
        }

        public static FixedVector<TDimension> SafeNormalize(FixedVector<TDimension> vector, double tolerance = 1e-6)
        {
            try
            {
                return Normalize(vector, tolerance);
            }
            catch (InvalidOperationException)
            {
                return Zeros();
            }
        }

        public static FixedVector<TDimension> Negate(FixedVector<TDimension> vector)
        {
            return new FixedVector<TDimension>(vector.Vector.Negate());
        }

        public static bool AlmostEqual(FixedVector<TDimension> a, FixedVector<TDimension> b, double maximumAbsoluteError = 1e-6)
        {
            return a.Vector.AlmostEqual(b.Vector, maximumAbsoluteError);
        }

        public static bool IsZero(FixedVector<TDimension> vector, double tolerance = 1e-6)
        {
            return vector.L2Norm().AlmostEqual(0.0, tolerance);
        }

        public static bool IsFinite(FixedVector<TDimension> vector)
        {
            return vector.Vector.All(coord => coord.IsFinite());
        }

        public static bool IsLessThan(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return a.L2Norm() < b.L2Norm();
        }

        public static bool IsGreaterThan(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return a.L2Norm() > b.L2Norm();
        }

        public static bool IsNearerThan(FixedVector<TDimension> a, FixedVector<TDimension> b, FixedVector<TDimension> origin)
        {
            return (a - origin).L2Norm() <= (b - origin).L2Norm();
        }

        public static bool IsFartherThan(FixedVector<TDimension> a, FixedVector<TDimension> b, FixedVector<TDimension> origin)
        {
            return (a - origin).L2Norm() >= (b - origin).L2Norm();
        }

        public static double Angle(FixedVector<TDimension> a, FixedVector<TDimension> b, double tolerance = 1e-6)
        {
            double lengthA = a.L2Norm();
            double lengthB = b.L2Norm();
            
            if (lengthA <= tolerance || lengthB <= tolerance)
                throw new InvalidOperationException("Cannot calculate angle for zero-length vector");

            double cos = a.Dot(b) / (lengthA * lengthB);
            cos = Math.Max(-1.0, Math.Min(1.0, cos));
            
            return Math.Acos(cos);
        }

        public static double DistanceTo(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return Math.Sqrt(DistanceSquaredTo(a, b));
        }

        public static double DistanceSquaredTo(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            FixedVector<TDimension> delta = b - a;
            return delta.Dot(delta);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((FixedVector<TDimension>)obj);
        }

        public override int GetHashCode()
        {
            return Vector != null ? Vector.GetHashCode() : 0;
        }
    }

    public static class FixedVectorExtensions
    {
        public static FixedVector<TDimension> Sum<TDimension>(this IEnumerable<FixedVector<TDimension>> vectors)
            where TDimension : struct, IDimension
        {
            return vectors.Aggregate(FixedVector<TDimension>.Zeros(), (current, vector) => current + vector);
        }

        public static FixedVector<TDimension> Mean<TDimension>(this IEnumerable<FixedVector<TDimension>> vectors)
            where TDimension : struct, IDimension
        {
            IReadOnlyCollection<FixedVector<TDimension>> vectorsArr = vectors.ToArray();
            return vectorsArr.Sum() * (1.0 / vectorsArr.Count);
        }

        public static bool IsBetween<TDimension>(
            this FixedVector<TDimension> point, 
            FixedVector<TDimension> start, 
            FixedVector<TDimension> end, 
            double tolerance = 1e-6)
            where TDimension : struct, IDimension
        {
            FixedVector<TDimension> ab = end - start;
            FixedVector<TDimension> ap = point - start;

            double lengthSquared = ab.Dot(ab);
            if (lengthSquared <= tolerance * tolerance)
                return false;

            double t = ap.Dot(ab) / lengthSquared;
            if (t < 0.0 || t > 1.0)
                return false;

            FixedVector<TDimension> projection = start.Lerp(end, t);
            return (point - projection).L2Norm() <= tolerance;
        }

        public static FixedVector<TDimension> Lerp<TDimension>(this FixedVector<TDimension> start, FixedVector<TDimension> end, double t)
            where TDimension : struct, IDimension
        {
            return start + (end - start) * t;
        }
    }

    public static class FixedVector2Extensions
    {
        public static double GetX(this FixedVector<Dim2> vector)
        {
            return vector[0];
        }

        public static double GetY(this FixedVector<Dim2> vector)
        {
            return vector[1];
        }

        public static void SetX(this FixedVector<Dim2> vector, double value)
        {
            vector[0] = value;
        }

        public static void SetY(this FixedVector<Dim2> vector, double value)
        {
            vector[1] = value;
        }
        
        public static FixedVector<Dim2> X(this FixedVectorBuilder<Dim2> builder)
        {
            return builder.Dense(1, 0);
        }

        public static FixedVector<Dim2> Y(this FixedVectorBuilder<Dim2> builder)
        {
            return builder.Dense(0, 1);
        }
        
        public static FixedVector<Dim2> Dense(this FixedVectorBuilder<Dim2> builder, double x)
        {
            return builder.Dense(x, 0);
        }

        public static FixedVector<Dim2> Dense(this FixedVectorBuilder<Dim2> builder, double x, double y)
        {
            return builder.Dense(x, y);
        }
    }

    public static class FixedVector3Extensions
    {
        public static double GetX(this FixedVector<Dim3> vector)
        {
            return vector[0];
        }

        public static double GetY(this FixedVector<Dim3> vector)
        {
            return vector[1];
        }

        public static double GetZ(this FixedVector<Dim3> vector)
        {
            return vector[2];
        }

        public static void SetX(this FixedVector<Dim3> vector, double value)
        {
            vector[0] = value;
        }

        public static void SetY(this FixedVector<Dim3> vector, double value)
        {
            vector[1] = value;
        }

        public static void SetZ(this FixedVector<Dim3> vector, double value)
        {
            vector[2] = value;
        }

        public static FixedVector<Dim3> CreateNormalVector(this FixedVector<Dim3> vector)
        {
            FixedVector<Dim3> worldUp = FixedVector<Dim3>.Builder.Dense(0, 0, 1);
            if (vector.IsParallel(worldUp))
                worldUp = FixedVector<Dim3>.Builder.Dense(0, 1, 0);

            FixedVector<Dim3> temp = vector.CrossProduct(worldUp);
            return vector.CreateNormalVector(temp);
        }

        public static FixedVector<Dim3> CreateNormalVector(this FixedVector<Dim3> a, FixedVector<Dim3> b)
        {
            return a.CrossProduct(b);
        }

        public static FixedVector<Dim3> CrossProduct(this FixedVector<Dim3> a, FixedVector<Dim3> b)
        {
            return FixedVector<Dim3>.Builder.Dense(a[1] * b[2] - a[2] * b[1], -a[0] * b[2] + a[2] * b[0], a[0] * b[1] - a[1] * b[0]);
        }

        public static FixedVector<Dim4> ToHomogeneous(this FixedVector<Dim3> vector, double value)
        {
            return FixedVector<Dim4>.Builder.Dense(vector[0], vector[1], vector[2], value);
        }

        public static FixedVector<Dim3> X(this FixedVectorBuilder<Dim3> builder)
        {
            return builder.Dense(1, 0, 0);
        }

        public static FixedVector<Dim3> Y(this FixedVectorBuilder<Dim3> builder)
        {
            return builder.Dense(0, 1, 0);
        }

        public static FixedVector<Dim3> Z(this FixedVectorBuilder<Dim3> builder)
        {
            return builder.Dense(0, 0, 1);
        }

        public static FixedVector<Dim3> Dense(this FixedVectorBuilder<Dim3> builder, double x)
        {
            return builder.Dense(x, 0, 0);
        }

        public static FixedVector<Dim3> Dense(this FixedVectorBuilder<Dim3> builder, double x, double y)
        {
            return builder.Dense(x, y, 0);
        }

        public static FixedVector<Dim3> Dense(this FixedVectorBuilder<Dim3> builder, double x, double y, double z)
        {
            return builder.Dense(x, y, z);
        }
    }

    public static class FixedVector4Extensions
    {
        public static double GetX(this FixedVector<Dim4> vector)
        {
            return vector[0];
        }

        public static double GetY(this FixedVector<Dim4> vector)
        {
            return vector[1];
        }

        public static double GetZ(this FixedVector<Dim4> vector)
        {
            return vector[2];
        }

        public static double GetW(this FixedVector<Dim4> vector)
        {
            return vector[3];
        }

        public static void SetX(this FixedVector<Dim4> vector, double value)
        {
            vector[0] = value;
        }

        public static void SetY(this FixedVector<Dim4> vector, double value)
        {
            vector[1] = value;
        }

        public static void SetZ(this FixedVector<Dim4> vector, double value)
        {
            vector[2] = value;
        }

        public static void SetW(this FixedVector<Dim4> vector, double value)
        {
            vector[3] = value;
        }

        public static FixedVector<Dim3> ToCartesian(this FixedVector<Dim4> vector, double tolerance = 1e-12)
        {
            double w = vector[3];
            if (Math.Abs(w) <= tolerance)
                throw new InvalidOperationException("Cannot convert homogenous coordinates with W is zero.");
            
            return FixedVector<Dim3>.Builder.Dense(vector[0] / w, vector[1] / w, vector[2] / w);
        }

        public static FixedVector<Dim4> X(this FixedVectorBuilder<Dim4> builder)
        {
            return builder.Dense(1, 0, 0, 0);
        }

        public static FixedVector<Dim4> Y(this FixedVectorBuilder<Dim4> builder)
        {
            return builder.Dense(0, 1, 0, 0);
        }

        public static FixedVector<Dim4> Z(this FixedVectorBuilder<Dim4> builder)
        {
            return builder.Dense(0, 0, 1, 0);
        }

        public static FixedVector<Dim4> W(this FixedVectorBuilder<Dim4> builder)
        {
            return builder.Dense(0, 0, 0, 1);
        }

        public static FixedVector<Dim4> Dense(this FixedVectorBuilder<Dim4> builder, double x)
        {
            return builder.Dense(x, 0, 0, 0);
        }

        public static FixedVector<Dim4> Dense(this FixedVectorBuilder<Dim4> builder, double x, double y)
        {
            return builder.Dense(x, y, 0, 0);
        }

        public static FixedVector<Dim4> Dense(this FixedVectorBuilder<Dim4> builder, double x, double y, double z)
        {
            return builder.Dense(x, y, z, 0);
        }

        public static FixedVector<Dim4> Dense(this FixedVectorBuilder<Dim4> builder, double x, double y, double z, double w)
        {
            return builder.Dense(x, y, z, w);
        }
    }
}