using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Utils.Mathematics
{
    public class FixedMatrixBuilder<TDimension>
        where TDimension : struct, IDimension
    {
        private static int Dimension => Dimension<TDimension>.Dim;
        
        public FixedMatrix<TDimension> Dense()
        {
            return new FixedMatrix<TDimension>(Matrix<double>.Build.Dense(Dimension, Dimension));
        }

        public FixedMatrix<TDimension> Dense(params double[] values)
        {
            if (values.Length != Dimension * Dimension)
                throw new ArgumentException($"Expected the list of values with length {Dimension * Dimension}, but taken {values.Length}");
            
            return new FixedMatrix<TDimension>(Matrix<double>.Build.Dense(Dimension, Dimension, values));
        }

        public FixedMatrix<TDimension> Dense(double[,] values)
        {
            if (values.GetLength(0) != Dimension || values.GetLength(1) != Dimension)
                throw new ArgumentException($"Expected the list of values with size {Dimension}x{Dimension}, but taken {values.GetLength(0)}x{values.GetLength(1)}");

            return new FixedMatrix<TDimension>(Matrix<double>.Build.DenseOfArray(values));
        }

        public FixedMatrix<TDimension> Dense(params FixedVector<TDimension>[] vector)
        {
            if (vector.Length != Dimension)
                throw new ArgumentException($"Expected the list of vectors with length {Dimension}, but taken {vector.Length}");
            
            return new FixedMatrix<TDimension>(Matrix<double>.Build.DenseOfColumnVectors(vector.Select(vec => vec.Vector)));
        }
    }
    
    public class FixedMatrix<TDimension>
        where TDimension : struct, IDimension
    {
        public static int Dimension => Dimension<TDimension>.Dim;
        public static FixedMatrixBuilder<TDimension> Builder => new FixedMatrixBuilder<TDimension>();

        public Matrix<double> Matrix { get; }

        public double this[int row, int col]
        {
            get => Matrix[row, col];
            set => Matrix[row, col] = value;
        }
        
        public FixedMatrix(Matrix<double> matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));
            
            if (matrix.RowCount != Dimension || matrix.ColumnCount != Dimension)
                throw new ArgumentException($"Expected matrix {Dimension}x{Dimension}");
            
            Matrix = matrix;
        }

        public void SetSubMatrix<TOtherDimension>(FixedMatrix<TOtherDimension> other, int rowIndex, int colIndex)
            where TOtherDimension : struct, IDimension
        {
            int otherDim = FixedMatrix<TOtherDimension>.Dimension;
            
            if (Dimension < otherDim)
                throw new ArgumentOutOfRangeException(nameof(FixedMatrix<TOtherDimension>));
            
            if (rowIndex + otherDim >= Dimension || colIndex + otherDim >= Dimension)
                throw new ArgumentOutOfRangeException($"Resulted endCol || endRow is higher than matrix dimension: {Dimension}");
            
            Matrix.SetSubMatrix(rowIndex, colIndex, other.Matrix);
        }

        public void SetColumn(int col, FixedVector<TDimension> vector)
        {
            Matrix.SetColumn(col, vector.Vector);
        }

        public FixedVector<TDimension> GetColumn(int col)
        {
            return new FixedVector<TDimension>(Matrix.Column(col));
        }

        public FixedMatrix<TDimension> Transpose()
        {
            return new FixedMatrix<TDimension>(Matrix.Transpose());
        }

        public FixedMatrix<TDimension> Inverse()
        {
            return new FixedMatrix<TDimension>(Matrix.Inverse());
        }

        public FixedVector<TDimension> Multiply(FixedVector<TDimension> vector)
        {
            return Multiply(this, vector);
        }

        public FixedVector<TDimension> LeftMultiply(FixedVector<TDimension> vector)
        {
            return LeftMultiply(this, vector);
        }

        public FixedMatrix<TDimension> Multiply(FixedMatrix<TDimension> other)
        {
            return Multiply(this, other);
        }

        public double Determinant()
        {
            return Matrix.Determinant();
        }

        public override string ToString()
        {
            return Matrix.ToString();
        }

        public FixedMatrix<TDimension> Clone()
        {
            return new FixedMatrix<TDimension>(Matrix.Clone());
        }

        public static FixedMatrix<TDimension> Zeros()
        {
            return new FixedMatrix<TDimension>(Matrix<double>.Build.Dense(Dimension, Dimension, 0.0));
        }

        public static FixedMatrix<TDimension> Ones()
        {
            return new FixedMatrix<TDimension>(Matrix<double>.Build.Dense(Dimension, Dimension, 1.0));
        }

        public static FixedMatrix<TDimension> Identity()
        {
            return new FixedMatrix<TDimension>(Matrix<double>.Build.DenseIdentity(Dimension));
        }

        public static FixedMatrix<TDimension> operator+(FixedMatrix<TDimension> a, FixedMatrix<TDimension> b)
        {
            return Add(a, b);
        }

        public static FixedMatrix<TDimension> operator-(FixedMatrix<TDimension> a, FixedMatrix<TDimension> b)
        {
            return Subtract(a, b);
        }

        public static FixedMatrix<TDimension> operator*(FixedMatrix<TDimension> a, FixedMatrix<TDimension> b)
        {
            return Multiply(a, b);
        }

        public static FixedVector<TDimension> operator*(FixedMatrix<TDimension> a, FixedVector<TDimension> b)
        {
            return Multiply(a, b);
        }
        
        public static FixedVector<TDimension> operator*(FixedVector<TDimension> b, FixedMatrix<TDimension> a)
        {
            return LeftMultiply(a, b);
        }
        
        public static FixedMatrix<TDimension> Add(FixedMatrix<TDimension> a, FixedMatrix<TDimension> b)
        {
            return new FixedMatrix<TDimension>(a.Matrix.Add(b.Matrix));
        }

        public static FixedMatrix<TDimension> Subtract(FixedMatrix<TDimension> a, FixedMatrix<TDimension> b)
        {
            return new FixedMatrix<TDimension>(a.Matrix.Subtract(b.Matrix));
        }

        public static FixedMatrix<TDimension> Multiply(FixedMatrix<TDimension> a, FixedMatrix<TDimension> b)
        {
            return new FixedMatrix<TDimension>(a.Matrix.Multiply(b.Matrix));
        }

        public static FixedVector<TDimension> Multiply(FixedMatrix<TDimension> a, FixedVector<TDimension> b)
        {
            return new FixedVector<TDimension>(a.Matrix.Multiply(b.Vector));
        }
        
        public static FixedVector<TDimension> LeftMultiply(FixedMatrix<TDimension> a, FixedVector<TDimension> b)
        {
            return new FixedVector<TDimension>(a.Matrix.LeftMultiply(b.Vector));
        }
    }

    public static class FixedMatrix3Extensions
    {
        public static FixedVector<Dim3> GetX(this FixedMatrix<Dim3> matrix)
        {
            return matrix.GetColumn(0);
        }
        
        public static FixedVector<Dim3> GetY(this FixedMatrix<Dim3> matrix)
        {
            return matrix.GetColumn(1);
        }
        
        public static FixedVector<Dim3> GetZ(this FixedMatrix<Dim3> matrix)
        {
            return matrix.GetColumn(2);
        }
        
        public static void SetX(this FixedMatrix<Dim3> matrix, FixedVector<Dim3> vector)
        {
            matrix.SetColumn(0, vector);
        }
        
        public static void SetY(this FixedMatrix<Dim3> matrix, FixedVector<Dim3> vector)
        {
            matrix.SetColumn(1, vector);
        }
        
        public static void SetZ(this FixedMatrix<Dim3> matrix, FixedVector<Dim3> vector)
        {
            matrix.SetColumn(2, vector);
        }

        public static FixedMatrix<Dim3> CreateTransition(this FixedMatrixBuilder<Dim3> builder,
            FixedVector<Dim3> xAxis, FixedVector<Dim3> yAxis, FixedVector<Dim3> zAxis)
        {
            FixedMatrix<Dim3> matrix = builder.Dense();
            matrix.SetX(xAxis);
            matrix.SetY(yAxis);
            matrix.SetZ(zAxis);

            return matrix;
        }

        public static FixedMatrix<Dim3> CreateTransition(this FixedMatrixBuilder<Dim3> builder, FixedVector<Dim3> zAxis)
        {
            FixedVector<Dim3> zAxisNorm = zAxis.Normalize();
            FixedVector<Dim3> xAxisNorm = zAxisNorm.CreateNormalVector().Normalize();
            FixedVector<Dim3> yAxisNorm = zAxisNorm.CreateNormalVector(xAxisNorm).Normalize();
            
            FixedMatrix<Dim3> matrix = builder.Dense();
            matrix.SetX(xAxisNorm);
            matrix.SetZ(yAxisNorm);
            matrix.SetZ(zAxisNorm);

            return matrix;
        }

        public static FixedMatrix<Dim3> CreateTransition(this FixedMatrixBuilder<Dim3> builder, FixedVector<Dim3> xAxis, FixedVector<Dim3> yAxis)
        {
            FixedVector<Dim3> xAxisNorm = xAxis.Normalize();
            FixedVector<Dim3> yAxisNorm = yAxis.Normalize();
            FixedVector<Dim3> zAxisNorm = xAxisNorm.CreateNormalVector(yAxisNorm).Normalize();

            FixedMatrix<Dim3> matrix = builder.Dense();
            matrix.SetX(xAxisNorm);
            matrix.SetY(yAxisNorm);
            matrix.SetZ(zAxisNorm);

            return matrix;
        }
    }
    
    public static class FixedMatrix4Extensions
    {
        public static FixedVector<Dim4> GetX(this FixedMatrix<Dim4> matrix)
        {
            return matrix.GetColumn(0);
        }
        
        public static FixedVector<Dim4> GetY(this FixedMatrix<Dim4> matrix)
        {
            return matrix.GetColumn(1);
        }
        
        public static FixedVector<Dim4> GetZ(this FixedMatrix<Dim4> matrix)
        {
            return matrix.GetColumn(2);
        }

        public static FixedVector<Dim4> GetOffset(this FixedMatrix<Dim4> matrix)
        {
            return matrix.GetColumn(3);
        }
        
        public static void SetX(this FixedMatrix<Dim4> matrix, FixedVector<Dim4> vector)
        {
            matrix.SetColumn(0, vector);
        }
        
        public static void SetY(this FixedMatrix<Dim4> matrix, FixedVector<Dim4> vector)
        {
            matrix.SetColumn(1, vector);
        }
        
        public static void SetZ(this FixedMatrix<Dim4> matrix, FixedVector<Dim4> vector)
        {
            matrix.SetColumn(2, vector);
        }

        public static void SetOffset(this FixedMatrix<Dim4> matrix, FixedVector<Dim4> vector)
        {
            matrix.SetColumn(3, vector);
        }

        public static FixedVector<Dim3> GetTranslation(this FixedMatrix<Dim4> matrix)
        {
            return matrix.GetOffset().ToCartesian();
        }

        public static FixedMatrix<Dim3> GetRotation(this FixedMatrix<Dim4> matrix)
        {
            return new FixedMatrix<Dim3>(matrix.Matrix.SubMatrix(0, 3, 0, 3));
        }
        
        public static FixedMatrix<Dim4> CreateTransition(this FixedMatrixBuilder<Dim4> builder, FixedVector<Dim4> offset)
        {
            FixedVector<Dim4> xAxis = FixedVector<Dim4>.Builder.X();
            FixedVector<Dim4> yAxis = FixedVector<Dim4>.Builder.Y();
            FixedVector<Dim4> zAxis = FixedVector<Dim4>.Builder.Z();

            return builder.CreateTransition(offset, xAxis, yAxis, zAxis);
        }

        public static FixedMatrix<Dim4> CreateTransition(this FixedMatrixBuilder<Dim4> builder, FixedVector<Dim3> offset)
        {
            FixedVector<Dim4> offsetNorm = offset.ToHomogeneous(1);
            return builder.CreateTransition(offsetNorm);
        }

        public static FixedMatrix<Dim4> CreateTransition(this FixedMatrixBuilder<Dim4> builder, 
            FixedVector<Dim4> offset, FixedVector<Dim4> xAxis, FixedVector<Dim4> yAxis, FixedVector<Dim4> zAxis)
        {
            FixedMatrix<Dim4> matrix = builder.Dense(xAxis, yAxis, zAxis, offset);
            return matrix;
        }
        
        public static FixedMatrix<Dim4> CreateTransition(this FixedMatrixBuilder<Dim4> builder,
            FixedVector<Dim3> offset, FixedVector<Dim3> xAxis, FixedVector<Dim3> yAxis, FixedVector<Dim3> zAxis)
        {
            FixedVector<Dim4> xAxisNorm = xAxis.ToHomogeneous(0);
            FixedVector<Dim4> yAxisNorm = yAxis.ToHomogeneous(0);
            FixedVector<Dim4> zAxisNorm = zAxis.ToHomogeneous(0);
            FixedVector<Dim4> offsetNorm = offset.ToHomogeneous(1);

            return builder.CreateTransition(offsetNorm, xAxisNorm, yAxisNorm, zAxisNorm);
        }
    }
}