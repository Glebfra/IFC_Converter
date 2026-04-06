using System;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Utils
{
    public static class MatrixExtensions
    {
        /// <summary>
        ///     Creates a rotation matrix representing a rotation around the X-axis in 3D space.
        ///     The method calculates the cosine and sine of the provided rotation angle and
        ///     constructs a 4x4 homogeneous transformation matrix. In this matrix, the X-axis
        ///     remains unchanged while the Y and Z axes are rotated in the YZ plane according
        ///     to the specified angle. The resulting matrix can be used to rotate points or
        ///     vectors around the X-axis while preserving homogeneous transformation compatibility,
        ///     making it suitable for use in geometric transformations, computer graphics,
        ///     CAD systems, and other 3D mathematical computations.
        /// </summary>
        /// <param name="angle">Rotation angle in radians.</param>
        /// <returns>
        ///     A 4x4 homogeneous rotation matrix that performs a rotation around the X-axis.
        /// </returns>
        [Pure]
        public static Matrix<double> CreateRotationAroundX(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            return DenseMatrix.OfArray(new[,]
            {
                { 1, 0, 0, 0 },
                { 0, cos, -sin, 0 },
                { 0, sin, cos, 0 },
                { 0, 0, 0, 1 }
            });
        }

        /// <summary>
        ///     Creates a rotation matrix representing a rotation around the Y-axis in 3D space.
        ///     The method computes the cosine and sine of the specified rotation angle and builds
        ///     a 4x4 homogeneous transformation matrix. In this matrix, the Y-axis remains fixed
        ///     while the X and Z axes rotate within the XZ plane according to the given angle.
        ///     The resulting matrix can be applied to transform points, vectors, or coordinate
        ///     systems in three-dimensional space and is commonly used in geometric modeling,
        ///     computer graphics, and spatial transformations.
        /// </summary>
        /// <param name="angle">Rotation angle in radians.</param>
        /// <returns>
        ///     A 4x4 homogeneous rotation matrix that performs a rotation around the Y-axis.
        /// </returns>
        [Pure]
        public static Matrix<double> CreateRotationAroundY(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            return DenseMatrix.OfArray(new[,]
            {
                { cos, 0, sin, 0 },
                { 0, 1, 0, 0 },
                { -sin, 0, cos, 0 },
                { 0, 0, 0, 1 }
            });
        }

        /// <summary>
        ///     Creates a rotation matrix representing a rotation around the Z-axis in 3D space.
        ///     The method evaluates the cosine and sine of the specified rotation angle and
        ///     constructs a 4x4 homogeneous transformation matrix. In this matrix, the Z-axis
        ///     remains unchanged while the X and Y axes rotate within the XY plane according
        ///     to the provided angle. The resulting matrix can be used to rotate points,
        ///     vectors, or entire coordinate systems around the Z-axis, which is a common
        ///     operation in computer graphics, geometric transformations, and CAD systems.
        /// </summary>
        /// <param name="angle">Rotation angle in radians.</param>
        /// <returns>
        ///     A 4x4 homogeneous rotation matrix that performs a rotation around the Z-axis.
        /// </returns>
        [Pure]
        public static Matrix<double> CreateRotationAroundZ(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            return DenseMatrix.OfArray(new[,]
            {
                { cos, -sin, 0, 0 },
                { sin, cos, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            });
        }

        /// <summary>
        ///     Create a 4x4 identity matrix, which is commonly used as the default transformation matrix in 3D graphics and
        ///     geometry processing.
        /// </summary>
        /// <returns>4x4 Matrix</returns>
        [Pure]
        public static Matrix<double> Identity()
        {
            return Matrix<double>.Build.DenseIdentity(4);
        }

        /// <summary>
        ///     Returns the X axis of the transformation matrix, which represents the right direction in a 3D coordinate system.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> GetX(this Matrix<double> matrix)
        {
            return matrix.Row(0).SubVector(0, 3);
        }


        /// <summary>
        ///     Returns the Y axis of the transformation matrix, which represents the up direction in a 3D coordinate system.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> GetY(this Matrix<double> matrix)
        {
            return matrix.Row(1).SubVector(0, 3);
        }


        /// <summary>
        ///     Returns the Z axis of the transformation matrix, which represents the forward direction in a 3D coordinate system.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> GetZ(this Matrix<double> matrix)
        {
            return matrix.Row(2).SubVector(0, 3);
        }


        /// <summary>
        ///     Returns the offset (translation) component of the transformation matrix, which represents the position of the
        ///     object in 3D space.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> GetOffset(this Matrix<double> matrix)
        {
            return matrix.Column(3).SubVector(0, 3);
        }

        /// <summary>
        ///     Sets the X axis of the transformation matrix, which represents the right direction in a 3D coordinate system.
        /// </summary>
        /// <param name="matrix">4x4 Matrix</param>
        /// <param name="vector">3D Vector</param>
        public static void SetX(this Matrix<double> matrix, Vector<double> vector)
        {
            matrix.SetRow(0, 0, 3, vector);
        }

        /// <summary>
        ///     Sets the Y axis of the transformation matrix, which represents the up direction in a 3D coordinate system.
        /// </summary>
        /// <param name="matrix">4x4 Matrix</param>
        /// <param name="vector">3D Vector</param>
        public static void SetY(this Matrix<double> matrix, Vector<double> vector)
        {
            matrix.SetRow(1, 0, 3, vector);
        }

        /// <summary>
        ///     Sets the Z axis of the transformation matrix, which represents the forward direction in a 3D coordinate system.
        /// </summary>
        /// <param name="matrix">4x4 Matrix</param>
        /// <param name="vector">3D Vector</param>
        public static void SetZ(this Matrix<double> matrix, Vector<double> vector)
        {
            matrix.SetRow(2, 0, 3, vector);
        }


        /// <summary>
        ///     Sets the offset of the transformation matrix, which represents the offset in a 3D coordinate system.
        /// </summary>
        /// <param name="matrix">4x4 Matrix</param>
        /// <param name="vector">3D Vector</param>
        public static void SetOffset(this Matrix<double> matrix, Vector<double> vector)
        {
            matrix.SetColumn(3, 0, 3, vector);
        }

        /// <summary>
        ///     Returns the right axis of the transformation matrix, which represents the right direction in a 3D coordinate
        ///     system.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> GetRight(this Matrix<double> matrix)
        {
            return matrix.GetX();
        }

        /// <summary>
        ///     Returns the up axis of the transformation matrix, which represents the up direction in a 3D coordinate system.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> GetUp(this Matrix<double> matrix)
        {
            return matrix.GetY();
        }

        /// <summary>
        ///     Returns the forward axis of the transformation matrix, which represents the forward direction in a 3D coordinate
        ///     system.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> GetForward(this Matrix<double> matrix)
        {
            return matrix.GetZ();
        }

        [Pure]
        public static Matrix<double> GetRotation(this Matrix<double> matrix)
        {
            return matrix.SubMatrix(0, 3, 0, 3);
        }

        /// <summary>
        ///     Creates a transition matrix, which is a 4x4 matrix that represents a transformation in 3D space, including rotation
        ///     and translation. The transition matrix can be used to transform points and vectors from one coordinate system to
        ///     another.
        /// </summary>
        /// <returns>4x4 Matrix</returns>
        [Pure]
        public static Matrix<double> CreateTransition()
        {
            return Identity();
        }

        /// <summary>
        ///     Applies the rotation component of the transformation matrix to a given vector, which is a 3D vector that represents
        ///     a point or direction in 3D space. The method multiplies the rotation part of the matrix (the upper-left 3x3
        ///     submatrix) with the input vector, resulting in a new vector that has been rotated according to the transformation
        ///     defined by the matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> ApplyRotation(this Matrix<double> matrix, Vector<double> vector)
        {
            return matrix.GetRotation().LeftMultiply(vector);
        }

        /// <summary>
        ///     Applies the translation component of the transformation matrix to a given vector, which is a 3D vector that
        ///     represents a point or direction in 3D space. The method adds the offset (translation) part of the matrix (the
        ///     fourth column) to the input vector, resulting in a new vector that has been translated according to the
        ///     transformation defined by the matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> ApplyOffset(this Matrix<double> matrix, Vector<double> vector)
        {
            return matrix.GetOffset() + vector;
        }

        /// <summary>
        ///     Creates a transition matrix with the specified position, which is a 3D vector that represents the translation
        ///     component of the transformation. The method initializes a 4x4 identity matrix and then sets the offset
        ///     (translation) part of the matrix to the provided position vector. The resulting matrix can be used to represent a
        ///     pure translation transformation in 3D space.
        /// </summary>
        /// <param name="position">3D Vector</param>
        /// <returns>4x4 Matrix</returns>
        [Pure]
        public static Matrix<double> CreateTransition(Vector<double> position)
        {
            Matrix<double> matrix = Identity();
            matrix.SetOffset(position);
            return matrix;
        }

        /// <summary>
        ///     Creates a transition matrix with the specified position, which is a 3D vector that represents the translation
        ///     component of the transformation. The method initializes a 4x4 identity matrix and then sets the offset
        ///     (translation) part of the matrix to the provided position vector. The resulting matrix can be used to represent a
        ///     pure translation transformation in 3D space.
        /// </summary>
        /// <param name="position">3D Vector</param>
        /// <param name="z">3D Vector</param>
        /// <returns>4x4 Matrix</returns>
        [Pure]
        public static Matrix<double> CreateTransitionWithWorldUp(Vector<double> position, Vector<double> z)
        {
            Vector<double> xm = z.Normalize(2);

            Vector<double> firstArbitraryVector = VectorExtensions.Z;
            if (firstArbitraryVector.IsParallel(xm))
                firstArbitraryVector = VectorExtensions.Y.Negate();
            Vector<double> secondArbitraryVector = xm.CrossProduct(firstArbitraryVector).Normalize(2);

            Vector<double> zm = xm.CrossProduct(secondArbitraryVector).Normalize(2);
            if (zm.DotProduct(VectorExtensions.Z) < 0)
                zm = zm.Negate();

            Vector<double> ym = zm.CrossProduct(xm).Normalize(2);

            return CreateTransition(position, ym, zm, xm);
        }

        /// <summary>
        ///     Creates a transition matrix with the specified position, which is a 3D vector that represents the translation
        ///     component of the transformation. The method initializes a 4x4 identity matrix and then sets the offset
        ///     (translation) part of the matrix to the provided position vector. The resulting matrix can be used to represent a
        ///     pure translation transformation in 3D space.
        /// </summary>
        /// <param name="position">3D Vector</param>
        /// <param name="z">3D Vector</param>
        /// <returns>4x4 Matrix</returns>
        [Pure]
        public static Matrix<double> CreateTransition(Vector<double> position, Vector<double> z)
        {
            Vector<double> x = z.CreateNormalVector();
            Vector<double> y = z.CrossProduct(x);
            return CreateTransition(position, x, y, z);
        }

        /// <summary>
        ///     Creates a transition matrix with the specified position, which is a 3D vector that represents the translation
        ///     component of the transformation. The method initializes a 4x4 identity matrix and then sets the offset
        ///     (translation) part of the matrix to the provided position vector. The resulting matrix can be used to represent a
        ///     pure translation transformation in 3D space.
        /// </summary>
        /// <param name="position">3D Vector</param>
        /// <param name="x">3D Vector</param>
        /// <param name="y">3D Vector</param>
        /// <returns>4x4 Matrix</returns>
        [Pure]
        public static Matrix<double> CreateTransition(Vector<double> position, Vector<double> x, Vector<double> y)
        {
            Vector<double> z = x.CrossProduct(y);
            return CreateTransition(position, x, y, z);
        }

        /// <summary>
        ///     Creates a transition matrix with the specified position, which is a 3D vector that represents the translation
        ///     component of the transformation. The method initializes a 4x4 identity matrix and then sets the offset
        ///     (translation) part of the matrix to the provided position vector. The resulting matrix can be used to represent a
        ///     pure translation transformation in 3D space.
        /// </summary>
        /// <param name="position">3D Vector</param>
        /// <param name="x">3D Vector</param>
        /// <param name="y">3D Vector</param>
        /// <param name="z">3D Vector</param>
        /// <returns>4x4 Matrix</returns>
        [Pure]
        public static Matrix<double> CreateTransition(Vector<double> position, Vector<double> x, Vector<double> y,
            Vector<double> z)
        {
            Matrix<double> matrix = Identity();
            matrix.SetOffset(position);
            matrix.SetX(x.Normalize(2));
            matrix.SetY(y.Normalize(2));
            matrix.SetZ(z.Normalize(2));
            return matrix;
        }

        public static string ToRowString(this Matrix<double> matrix)
        {
            return
                $"({matrix.GetX().ToRowString()}); ({matrix.GetY().ToRowString()}); ({matrix.GetZ().ToRowString()}); ({matrix.GetOffset().ToRowString()})";
        }
    }
}