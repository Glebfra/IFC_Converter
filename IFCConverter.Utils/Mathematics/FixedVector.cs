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

        public FixedVector<TDimension> Dense(params double[] values)
        {
            if (values.Length != Dimension)
                throw new ArgumentException($"The vector length {values.Length} is not equal to the dimension {Dimension}.");

            return new FixedVector<TDimension>(Vector<double>.Build.Dense(values));
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
            return Equals(Vector, other.Vector);
        }

        public double Dot(FixedVector<TDimension> other)
        {
            return Dot(this, other);
        }

        public double L2Norm()
        {
            return L2Norm(this);
        }

        public bool IsParallel(FixedVector<TDimension> vector, double maximumAbsoluteError = 1e-6)
        {
            return IsParallel(this, vector, maximumAbsoluteError);
        }

        public bool IsNormal(FixedVector<TDimension> b, double maximumAbsoluteError = 1e-6)
        {
            return IsNormal(this, b, maximumAbsoluteError);
        }

        public FixedVector<TDimension> Normalize()
        {
            return Normalize(this);
        }

        public FixedVector<TDimension> Negate()
        {
            return Negate(this);
        }

        public bool AlmostEqual(FixedVector<TDimension> other, double maximumAbsoluteError = 1e-6)
        {
            return AlmostEqual(this, other, maximumAbsoluteError);
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
            return Math.Abs(a.Dot(b)) < maximumAbsoluteError;
        }

        public static FixedVector<TDimension> Normalize(FixedVector<TDimension> vector)
        {
            return new FixedVector<TDimension>(vector.Vector.Normalize(2));
        }

        public static FixedVector<TDimension> Negate(FixedVector<TDimension> vector)
        {
            return new FixedVector<TDimension>(vector.Vector.Negate());
        }

        public static bool AlmostEqual(FixedVector<TDimension> a, FixedVector<TDimension> b, double maximumAbsoluteError = 1e-6)
        {
            return a.Vector.AlmostEqual(b.Vector, maximumAbsoluteError);
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

        public static double Angle(FixedVector<TDimension> a, FixedVector<TDimension> b)
        {
            return a.Vector.Angle(b.Vector);
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

        public static FixedVector<TDimension> Average<TDimension>(this IEnumerable<FixedVector<TDimension>> vectors)
            where TDimension : struct, IDimension
        {
            return vectors.Sum() * (1 / vectors.Count());
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
            return new FixedVector<Dim3>(a.Vector.CrossProduct(b.Vector));
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

        public static FixedVector<Dim3> ToCartesian(this FixedVector<Dim4> vector)
        {
            double delimiter = vector[3] == 0 ? 1 : vector[3];
            return FixedVector<Dim3>.Builder.Dense(vector[0] / delimiter, vector[1] / delimiter, vector[2] / delimiter);
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