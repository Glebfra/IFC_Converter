using System;
using System.Linq;
using IFCConverter.Utils.Mathematics;
using NUnit.Framework;

namespace IFCConverter.Tests.Mathematics
{
    [TestFixture]
    public sealed class FixedMatrixExtensionsTests
    {
        private const double Tolerance = 1e-10;

        private static FixedVector<Dim3> Vector3(double x, double y, double z)
        {
            return FixedVector<Dim3>.Builder.Dense(x, y, z);
        }

        private static void AssertVectorEqual<TDimension>(FixedVector<TDimension> actual, params double[] expected)
            where TDimension : struct, IDimension
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Count(), Is.EqualTo(expected.Length));

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.That(
                    actual[i],
                    Is.EqualTo(expected[i]).Within(Tolerance),
                    $"Unexpected vector component at index {i}.");
            }
        }

        private static void AssertMatrixEqual<TDimension>(FixedMatrix<TDimension> actual, double[,] expected)
            where TDimension : struct, IDimension
        {
            Assert.That(actual, Is.Not.Null);

            for (int row = 0; row < expected.GetLength(0); row++)
            {
                for (int column = 0; column < expected.GetLength(1); column++)
                {
                    Assert.That(
                        actual[row, column],
                        Is.EqualTo(expected[row, column]).Within(Tolerance),
                        $"Unexpected matrix value at [{row}, {column}].");
                }
            }
        }

        [Test]
        public void CreateRotationAroundX_RotatesYAxisToZAxis()
        {
            FixedMatrix<Dim3> rotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundX(Math.PI / 2);

            AssertVectorEqual(rotation * Vector3(0, 1, 0), 0, 0, 1);
        }

        [Test]
        public void CreateRotationAroundX_LeavesXAxisUnchanged()
        {
            FixedMatrix<Dim3> rotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundX(Math.PI / 2);

            AssertVectorEqual(rotation * Vector3(1, 0, 0), 1, 0, 0);
        }

        [Test]
        public void CreateRotationAroundY_RotatesXAxisToNegativeZAxis()
        {
            FixedMatrix<Dim3> rotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundY(Math.PI / 2);

            AssertVectorEqual(rotation * Vector3(1, 0, 0), 0, 0, -1);
        }

        [Test]
        public void CreateRotationAroundZ_RotatesXAxisToYAxis()
        {
            FixedMatrix<Dim3> rotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundZ(Math.PI / 2);

            AssertVectorEqual(rotation * Vector3(1, 0, 0), 0, 1, 0);
        }

        [Test]
        public void CreateRotationAroundZ_ZeroAngle_ReturnsIdentity()
        {
            FixedMatrix<Dim3> rotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundZ(0);

            AssertMatrixEqual(rotation, new[,]
            {
                { 1.0, 0.0, 0.0 },
                { 0.0, 1.0, 0.0 },
                { 0.0, 0.0, 1.0 }
            });
        }

        [Test]
        public void CreateRotationAroundZ_HalfTurn_ReversesXAxisAndYAxis()
        {
            FixedMatrix<Dim3> rotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundZ(Math.PI);

            AssertVectorEqual(rotation * Vector3(1, 0, 0), -1, 0, 0);
            AssertVectorEqual(rotation * Vector3(0, 1, 0), 0, -1, 0);
        }

        [TestCase(0.2, 0.3, 0.4)]
        [TestCase(1.0, -2.0, 3.0)]
        [TestCase(-5.0, 0.5, 2.5)]
        public void CreateRotationAroundZ_PreservesVectorLength(double x, double y, double z)
        {
            FixedVector<Dim3> vector = Vector3(x, y, z);
            FixedMatrix<Dim3> rotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundZ(0.73);

            FixedVector<Dim3> rotated = rotation * vector;

            Assert.That(
                rotated.L2Norm(),
                Is.EqualTo(vector.L2Norm()).Within(Tolerance));
        }

        [Test]
        public void CreateRotationAroundZ_RotationAndInverseRestoreVector()
        {
            FixedVector<Dim3> vector = Vector3(1, 2, 3);

            FixedMatrix<Dim3> rotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundZ(0.73);

            FixedMatrix<Dim3> inverseRotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundZ(-0.73);

            AssertVectorEqual(inverseRotation * (rotation * vector), 1, 2, 3);
        }

        [Test]
        public void CreateRotationAroundVector_RotatesAroundUnitZAxis()
        {
            FixedVector<Dim3> axis = Vector3(0, 0, 1);

            FixedMatrix<Dim3> rotation = FixedMatrix<Dim3>.Builder.CreateRotationAroundVector(axis, Math.PI / 2);

            AssertVectorEqual(rotation * Vector3(1, 0, 0), 0, 1, 0);
        }

        [Test]
        public void CreateTransition_WithXAxisAndYAxis_CreatesOrthonormalBasis()
        {
            FixedVector<Dim3> xAxis = Vector3(1, 0, 0);
            FixedVector<Dim3> yAxis = Vector3(0, 1, 0);

            FixedMatrix<Dim3> transition = FixedMatrix<Dim3>.Builder.CreateTransition(xAxis, yAxis);

            FixedVector<Dim3> x = transition.GetX();
            FixedVector<Dim3> y = transition.GetY();
            FixedVector<Dim3> z = transition.GetZ();

            Assert.That(x.L2Norm(), Is.EqualTo(1).Within(Tolerance));
            Assert.That(y.L2Norm(), Is.EqualTo(1).Within(Tolerance));
            Assert.That(z.L2Norm(), Is.EqualTo(1).Within(Tolerance));

            Assert.That(x.IsNormal(y), Is.True);
            Assert.That(y.IsNormal(z), Is.True);
            Assert.That(z.IsNormal(x), Is.True);
        }

        [Test]
        public void CreateTransition_WithZAxis_CreatesOrthonormalBasis()
        {
            FixedVector<Dim3> zAxis = Vector3(0, 0, 1);

            FixedMatrix<Dim3> transition = FixedMatrix<Dim3>.Builder.CreateTransition(zAxis);

            FixedVector<Dim3> x = transition.GetX();
            FixedVector<Dim3> y = transition.GetY();
            FixedVector<Dim3> z = transition.GetZ();

            Assert.That(x.L2Norm(), Is.EqualTo(1).Within(Tolerance));
            Assert.That(y.L2Norm(), Is.EqualTo(1).Within(Tolerance));
            Assert.That(z.L2Norm(), Is.EqualTo(1).Within(Tolerance));

            Assert.That(x.IsNormal(y), Is.True);
            Assert.That(y.IsNormal(z), Is.True);
            Assert.That(z.IsNormal(x), Is.True);
        }

        [Test]
        public void CreateTransition_WithZAxis_PreservesSpecifiedZDirection()
        {
            FixedVector<Dim3> zAxis = Vector3(0, 0, 2);

            FixedMatrix<Dim3> transition = FixedMatrix<Dim3>.Builder.CreateTransition(zAxis);

            AssertVectorEqual(transition.GetZ(), 0, 0, 1);
        }

        [Test]
        public void GetRotation_ExtractsUpperLeftThreeByThreeMatrix()
        {
            FixedMatrix<Dim4> matrix = FixedMatrix<Dim4>.Builder.Dense(new[,]
            {
                { 1.0, 2.0, 3.0, 10.0 },
                { 4.0, 5.0, 6.0, 20.0 },
                { 7.0, 8.0, 9.0, 30.0 },
                { 0.0, 0.0, 0.0, 1.0 }
            });

            FixedMatrix<Dim3> rotation = matrix.GetRotation();

            AssertMatrixEqual(rotation, new[,]
            {
                { 1.0, 2.0, 3.0 },
                { 4.0, 5.0, 6.0 },
                { 7.0, 8.0, 9.0 }
            });
        }

        [Test]
        public void CreateTransition4_TransformsPointByTranslation()
        {
            FixedVector<Dim3> offset = Vector3(10, 20, 30);

            FixedMatrix<Dim4> transition = FixedMatrix<Dim4>.Builder.CreateTransition(offset);

            FixedVector<Dim4> point = Vector3(1, 2, 3).ToHomogeneous(1);
            FixedVector<Dim3> result = (transition * point).ToCartesian();

            AssertVectorEqual(result, 11, 22, 33);
        }

        [Test]
        public void CreateTransition4_DoesNotTranslateDirection()
        {
            FixedVector<Dim3> offset = Vector3(10, 20, 30);

            FixedMatrix<Dim4> transition = FixedMatrix<Dim4>.Builder.CreateTransition(offset);

            FixedVector<Dim4> direction = Vector3(1, 2, 3).ToHomogeneous(0);
            FixedVector<Dim4> result = transition * direction;

            AssertVectorEqual(result, 1, 2, 3, 0);
        }
    }
}