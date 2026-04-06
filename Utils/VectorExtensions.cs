using System;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Utils
{
    public static class VectorExtensions
    {
        /// <summary>
        ///     Represents the zero vector and the unit vectors along the X, Y, and Z axes, as well as their combinations.
        ///     These vectors can be used as reference points or directions in 3D space.
        /// </summary>
        public static Vector<double> Zero => new DenseVector(3);

        /// <summary>
        ///     Represents the unit vectors along the X, Y, and Z axes, as well as their combinations. These vectors can be
        ///     used as reference points or directions in 3D space.
        /// </summary>
        public static Vector<double> X => new DenseVector(new double[] { 1, 0, 0 });

        /// <summary>
        ///     Represents the unit vectors along the X, Y, and Z axes, as well as their combinations. These vectors can be
        ///     used as reference points or directions in 3D space.
        /// </summary>
        public static Vector<double> Y => new DenseVector(new double[] { 0, 1, 0 });

        /// <summary>
        ///     Represents the unit vectors along the X, Y, and Z axes, as well as their combinations. These vectors can be
        ///     used as reference points or directions in 3D space.
        /// </summary>
        public static Vector<double> Z => new DenseVector(new double[] { 0, 0, 1 });

        /// <summary>
        ///     Represents the unit vectors along the X, Y, and Z axes, as well as their combinations. These vectors can be
        ///     used as reference points or directions in 3D space.
        /// </summary>
        public static Vector<double> XY => new DenseVector(new double[] { 1, 1, 0 });

        /// <summary>
        ///     Represents the unit vectors along the X, Y, and Z axes, as well as their combinations. These vectors can be
        ///     used as reference points or directions in 3D space.
        /// </summary>
        public static Vector<double> YZ => new DenseVector(new double[] { 0, 1, 1 });

        /// <summary>
        ///     Represents the unit vectors along the X, Y, and Z axes, as well as their combinations. These vectors can be
        ///     used as reference points or directions in 3D space.
        /// </summary>
        public static Vector<double> XZ => new DenseVector(new double[] { 1, 0, 1 });

        /// <summary>
        ///     Represents the unit vectors along the X, Y, and Z axes, as well as their combinations. These vectors can be
        ///     used as reference points or directions in 3D space.
        /// </summary>
        public static Vector<double> XYZ => new DenseVector(new double[] { 1, 1, 1 });

        /// <summary>
        ///     Represents the right, up, and forward directions in a 3D coordinate system. These vectors can be used to
        ///     define the orientation of objects or to specify movement directions in 3D space.
        /// </summary>
        public static Vector<double> Right => X;

        /// <summary>
        ///     Represents the right, up, and forward directions in a 3D coordinate system. These vectors can be used to
        ///     define the orientation of objects or to specify movement directions in 3D space.
        /// </summary>
        public static Vector<double> Up => Y;

        /// <summary>
        ///     Represents the right, up, and forward directions in a 3D coordinate system. These vectors can be used to
        ///     define the orientation of objects or to specify movement directions in 3D space.
        /// </summary>
        public static Vector<double> Forward => Z;

        /// <summary>
        ///     Returns a new vector that is the homogenous representation of the original vector. The homogenous representation
        ///     is commonly used in computer graphics and geometric transformations, where an additional coordinate
        ///     (the fourth coordinate) is added to represent points in 3D space. The original vector's X, Y, and Z components
        ///     are preserved, and the fourth component is set to 1, allowing for translations and other transformations to
        ///     be applied using matrix operations.
        /// </summary>
        /// <param name="vector">3D Vector</param>
        /// <returns>4D Vector</returns>
        [Pure]
        public static Vector<double> ToHomogenous(this Vector<double> vector)
        {
            return new DenseVector(new[] { vector[0], vector[1], vector[2], 1 });
        }

        /// <summary>
        ///     Returns a new vector that is the Cartesian representation of the original homogenous vector. The Cartesian
        ///     representation is obtained by dividing the X, Y, and Z components of the homogenous vector by its fourth component
        ///     (the homogenous coordinate). This conversion is essential for interpreting the homogenous coordinates as points
        ///     in 3D space, allowing for accurate geometric transformations and calculations.
        /// </summary>
        /// <param name="vector">4D Vector</param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> ToCartesian(this Vector<double> vector)
        {
            return vector.SubVector(0, 3) / vector[3];
        }

        /// <summary>
        ///     Returns the X component of the vector. This method provides a convenient way to access the X coordinate of a
        ///     3D vector, which is often used in various geometric calculations and transformations in 3D space.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>X Component</returns>
        [Pure]
        public static double GetX(this Vector<double> vector)
        {
            return vector[0];
        }

        /// <summary>
        ///     Returns the Y component of the vector. This method provides a convenient way to access the Y coordinate of a
        ///     3D vector, which is often used in various geometric calculations and transformations in 3D space.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>Y Component</returns>
        [Pure]
        public static double GetY(this Vector<double> vector)
        {
            return vector[1];
        }

        /// <summary>
        ///     Returns the Z component of the vector. This method provides a convenient way to access the Z coordinate of a
        ///     3D vector, which is often used in various geometric calculations and transformations in 3D space.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>Z Component</returns>
        [Pure]
        public static double GetZ(this Vector<double> vector)
        {
            return vector[2];
        }

        /// <summary>
        ///     Returns true if the two vectors are parallel to each other within a specified tolerance. The method normalizes
        ///     both vectors and calculates their cross product. If the L2 norm of the cross product is less than the given
        ///     tolerance, it indicates that the vectors are parallel. This is a common technique in vector mathematics to
        ///     determine
        ///     parallelism, as parallel vectors will have a cross product that is close to zero.
        /// </summary>
        /// <param name="first">First vector</param>
        /// <param name="second">Second vector</param>
        /// <param name="tolerance">Maximum error</param>
        /// <returns>True if vectors are parallel. Otherwise false</returns>
        [Pure]
        public static bool IsParallel(this Vector<double> first, Vector<double> second, double tolerance = 1e-6)
        {
            return first.Normalize(2).CrossProduct(second.Normalize(2)).L2Norm() < tolerance;
        }

        /// <summary>
        ///     Returns true if the two vectors are normal to each other within a specified tolerance. The method normalizes
        ///     both vectors and calculates their dot product. If the dot product is less than the given tolerance, it
        ///     indicates that the vectors are normal. This is a common technique in vector mathematics to determine
        ///     parallelism, as parallel vectors will have a cross product that is close to zero.
        /// </summary>
        /// <param name="first">First vector</param>
        /// <param name="second">Second vector</param>
        /// <param name="tolerance">Maximum error</param>
        /// <returns>True if vectors are normal. Otherwise false</returns>
        [Pure]
        public static bool IsNormal(this Vector<double> first, Vector<double> second, double tolerance = 1e-6)
        {
            return Math.Abs(first.DotProduct(second)) < tolerance;
        }

        /// <summary>
        ///     Calculates the cross product of two vectors. The cross product is a vector that is perpendicular to both
        ///     input vectors and has a magnitude equal to the area of the parallelogram formed by the input vectors. The
        ///     method computes the cross product using the standard formula for 3D vectors, where each component of the
        ///     resulting vector is calculated based on the components of the input vectors. This operation is fundamental in
        ///     various applications, including physics, engineering, and computer graphics, where it is used to determine
        ///     orientations, calculate torques, and perform other vector operations in 3D space.
        /// </summary>
        /// <param name="first">First 3D vector</param>
        /// <param name="second">Second 3D vector</param>
        /// <returns>3D vector</returns>
        [Pure]
        public static Vector<double> CrossProduct(this Vector<double> first, Vector<double> second)
        {
            Vector<double> res = new DenseVector(3);
            res[0] = first[1] * second[2] - first[2] * second[1];
            res[1] = -first[0] * second[2] + first[2] * second[0];
            res[2] = first[0] * second[1] - first[1] * second[0];
            return res;
        }

        /// <summary>
        ///     Creates a normal vector to the given vector. The method first checks if the input vector is parallel to the
        ///     world up vector (Z-axis). If it is, it uses the Y-axis as the world up vector instead. Then, it calculates a
        ///     temporary vector by taking the cross product of the input vector and the world up vector. Finally, it returns
        ///     the cross product of the input vector and the temporary vector, normalized to have a length of 1. This
        ///     resulting vector is perpendicular to the original input vector, making it a normal vector in 3D space. This
        ///     technique is commonly used in computer graphics and geometric computations to find a normal direction for
        ///     shading, lighting, or other purposes where perpendicular vectors are needed.
        /// </summary>
        /// <param name="vector">3D Vector</param>
        /// <returns>3D Vector</returns>
        [Pure]
        public static Vector<double> CreateNormalVector(this Vector<double> vector)
        {
            Vector<double> worldUp = Z;
            if (vector.IsParallel(worldUp))
                worldUp = Y;

            Vector<double> temp = vector.CrossProduct(worldUp);
            return CreateNormalVector(vector, temp);
        }

        /// <summary>
        ///     Creates a normal vector from two given vectors by calculating their cross product and normalizing the result.
        ///     The method computes a vector that is perpendicular to both input vectors in 3D space. First, it calculates the
        ///     cross product of the two vectors, which produces a vector orthogonal to the plane defined by them. Then, the
        ///     resulting vector is normalized so that its length becomes 1. The returned vector represents a unit normal to
        ///     the plane formed by the two input vectors. This operation is commonly used in computer graphics, geometric
        ///     modeling, and physics simulations for determining surface normals, constructing coordinate systems, or
        ///     computing orientations.
        /// </summary>
        /// <param name="first">First 3D vector.</param>
        /// <param name="second">Second 3D vector.</param>
        /// <returns>
        ///     A normalized 3D vector that is perpendicular to both <paramref name="first" /> and <paramref name="second" />.
        /// </returns>
        [Pure]
        public static Vector<double> CreateNormalVector(Vector<double> first, Vector<double> second)
        {
            return first.CrossProduct(second).Normalize(2);
        }

        /// <summary>
        ///     Calculates the angle between two vectors in radians. The method first normalizes both input vectors to have a
        ///     length of 1. Then, it computes the dot product of the normalized vectors and applies the arccosine function
        ///     to the result to obtain the angle in radians. This calculation is based on the geometric definition of the
        ///     dot product, which relates to the cosine of the angle between two vectors. The resulting angle can be used
        ///     in various applications, such as determining orientations, calculating rotations, or performing other
        ///     geometric operations in 3D space.
        /// </summary>
        /// <param name="first">First vector</param>
        /// <param name="second">Second vector</param>
        /// <returns>Angle between angles</returns>
        [Pure]
        public static double Angle(this Vector<double> first, Vector<double> second)
        {
            return Math.Acos(first.Normalize(2).DotProduct(second.Normalize(2)));
        }

        /// <summary>
        ///     Converts the given vector into a row-style string representation. The method iterates through
        ///     all elements of the vector and concatenates them into a single string separated by semicolons.
        ///     This format is useful for logging, debugging, exporting numeric data, or displaying vector
        ///     values in a compact and human-readable form. The resulting string represents the vector as
        ///     a sequence of scalar components arranged in row order.
        /// </summary>
        /// <param name="vector">Vector containing numeric components.</param>
        /// <returns>
        ///     A string containing all vector elements separated by semicolons.
        /// </returns>
        [Pure]
        public static string ToRowString(this Vector<double> vector)
        {
            return string.Join(";", vector);
        }
    }
}