using System;
using System.Linq;
using IFCConverter.Utils.Mathematics;
using NUnit.Framework;

namespace IFCConverter.Tests.Mathematics
{
    [TestFixture]
    public sealed class FixedVectorTests
    {
        private const double Tolerance = 1e-10;
        
        private static FixedVector<Dim2> Vector2(double x, double y)
        {
            return FixedVector<Dim2>.Builder.Dense(x, y);
        }

        private static FixedVector<Dim3> Vector3(double x, double y, double z)
        {
            return FixedVector<Dim3>.Builder.Dense(x, y, z);
        }

        private static void AssertVectorEqual<TDimension>(
            FixedVector<TDimension> actual,
            params double[] expected)
            where TDimension : struct, IDimension
        {
            Assert.That(actual.Count(), Is.EqualTo(expected.Length));

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.That(
                    actual[i],
                    Is.EqualTo(expected[i]).Within(Tolerance),
                    $"Unexpected value at index {i}.");
            }
        }

        [Test]
        public void BuilderDense_CreatesZeroVector()
        {
            AssertVectorEqual(FixedVector<Dim3>.Builder.Dense(), 0, 0, 0);
        }

        [Test]
        public void BuilderDense_WithScalar_FillsAllComponents()
        {
            AssertVectorEqual(FixedVector<Dim3>.Builder.Dense(2.5), 2.5, 2.5, 2.5);
        }

        [Test]
        public void BuilderDense_WithValues_CreatesVector()
        {
            AssertVectorEqual(Vector3(1, 2, 3), 1, 2, 3);
        }

        [Test]
        public void BuilderDense_WithIncorrectDimension_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => FixedVector<Dim3>.Builder.Dense(1, 2));
        }

        [Test]
        public void Indexer_CanModifyComponent()
        {
            FixedVector<Dim3> vector = Vector3(1, 2, 3);

            vector[1] = 10;

            AssertVectorEqual(vector, 1, 10, 3);
        }

        [Test]
        public void Enumeration_ReturnsComponentsInOrder()
        {
            Assert.That(Vector3(4, 5, 6).ToArray(), Is.EqualTo(new[] { 4.0, 5.0, 6.0 }));
        }

        [Test]
        public void Add_ReturnsComponentwiseSum()
        {
            AssertVectorEqual(Vector3(1, 2, 3) + Vector3(4, 5, 6), 5, 7, 9);
        }

        [Test]
        public void Subtract_ReturnsComponentwiseDifference()
        {
            AssertVectorEqual(Vector3(4, 5, 6) - Vector3(1, 2, 3), 3, 3, 3);
        }

        [Test]
        public void MultiplyByScalar_ReturnsScaledVector()
        {
            AssertVectorEqual(Vector3(1, -2, 3) * 2, 2, -4, 6);
        }

        [Test]
        public void MultiplyByScalarOnLeft_ReturnsScaledVector()
        {
            AssertVectorEqual(2 * Vector3(1, -2, 3), 2, -4, 6);
        }

        [Test]
        public void MultiplyTwoVectors_ReturnsDotProduct()
        {
            Assert.That(Vector3(1, 2, 3) * Vector3(4, 5, 6), Is.EqualTo(32).Within(Tolerance));
        }

        [Test]
        public void Dot_ReturnsCorrectValue()
        {
            Assert.That(Vector3(1, 2, 3).Dot(Vector3(4, 5, 6)), Is.EqualTo(32).Within(Tolerance));
        }
        
        [Test]
        public void Normalize_ZeroVector_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => FixedVector<Dim3>.Zeros().Normalize());
        }

        [Test]
        public void SafeNormalize_ZeroVector_EqualsZeros()
        {
            AssertVectorEqual(FixedVector<Dim3>.Zeros().SafeNormalize(), 0, 0, 0);
        }

        [Test]
        public void Negate_ReversesAllComponents()
        {
            AssertVectorEqual(Vector3(1, -2, 3).Negate(), -1, 2, -3);
        }

        [Test]
        public void IsParallel_ReturnsTrueForSameDirection()
        {
            Assert.That(Vector3(1, 2, 3).IsParallel(Vector3(2, 4, 6)), Is.True);
        }

        [Test]
        public void IsParallel_ReturnsTrueForOppositeDirections()
        {
            Assert.That(Vector3(1, 2, 3).IsParallel(Vector3(-2, -4, -6)), Is.True);
        }

        [Test]
        public void IsParallel_ReturnsFalseForPerpendicularVectors()
        {
            Assert.That(Vector3(1, 0, 0).IsParallel(Vector3(0, 1, 0)), Is.False);
        }

        [Test]
        public void IsParallel_ReturnsFalseWhenEitherVectorIsZero()
        {
            Assert.That(Vector3(0, 0, 0).IsParallel(Vector3(1, 0, 0)), Is.False);
        }

        [Test]
        public void IsNormal_ReturnsTrueForPerpendicularVectors()
        {
            Assert.That(Vector3(1, 0, 0).IsNormal(Vector3(0, 1, 0)), Is.True);
        }

        [Test]
        public void IsNormal_ReturnsFalseForNonPerpendicularVectors()
        {
            Assert.That(Vector3(1, 0, 0).IsNormal(Vector3(1, 1, 0)), Is.False);
        }

        [Test]
        public void AlmostEqual_ReturnsTrueWithinTolerance()
        {
            Assert.That(Vector2(1, 2).AlmostEqual(Vector2(1 + 1e-8, 2), 1e-6), Is.True);
        }

        [Test]
        public void AlmostEqual_ReturnsFalseOutsideTolerance()
        {
            Assert.That(Vector2(1, 2).AlmostEqual(Vector2(1.01, 2), 1e-6), Is.False);
        }

        [Test]
        public void IsLessThan_UsesVectorLength()
        {
            Assert.That(Vector3(1, 0, 0).IsLessThan(Vector3(0, 2, 0)), Is.True);
        }

        [Test]
        public void IsGreaterThan_UsesVectorLength()
        {
            Assert.That(Vector3(0, 2, 0).IsGreaterThan(Vector3(1, 0, 0)), Is.True);
        }

        [Test]
        public void IsNearerThan_UsesDistanceFromOrigin()
        {
            FixedVector<Dim3> origin = Vector3(0, 0, 0);

            Assert.That(Vector3(1, 0, 0).IsNearerThan(Vector3(2, 0, 0), origin), Is.True);
        }

        [Test]
        public void IsFartherThan_UsesDistanceFromOrigin()
        {
            FixedVector<Dim3> origin = Vector3(0, 0, 0);

            Assert.That(Vector3(2, 0, 0).IsFartherThan(Vector3(1, 0, 0), origin), Is.True);
        }

        [Test]
        public void Angle_PerpendicularVectors_ReturnsHalfPi()
        {
            Assert.That(
                Vector3(1, 0, 0).Angle(Vector3(0, 1, 0)),
                Is.EqualTo(Math.PI / 2).Within(Tolerance));
        }

        [Test]
        public void Angle_SameDirection_ReturnsZero()
        {
            Assert.That(
                Vector3(1, 0, 0).Angle(Vector3(2, 0, 0)),
                Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void Angle_OppositeDirections_ReturnsPi()
        {
            Assert.That(
                Vector3(1, 0, 0).Angle(Vector3(-1, 0, 0)),
                Is.EqualTo(Math.PI).Within(Tolerance));
        }

        [Test]
        public void Equals_ReturnsTrueForEqualVectors()
        {
            Assert.That(Vector3(1, 2, 3).Equals(Vector3(1, 2, 3)), Is.True);
        }

        [Test]
        public void Equals_ReturnsFalseForDifferentVectors()
        {
            Assert.That(Vector3(1, 2, 3).Equals(Vector3(1, 2, 4)), Is.False);
        }

        [Test]
        public void Dim3_Vector_HasThreeCoordinates()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            AssertVector(vector, 1.0, 2.0, 3.0);
        }

        [Test]
        public void Addition_ReturnsCoordinateWiseSum()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(4.0, 5.0, 6.0);

            FixedVector<Dim3> result = a + b;
            AssertVector(result, 5.0, 7.0, 9.0);
        }

        [Test]
        public void Subtraction_ReturnsCoordinateWiseDifference()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(5.0, 7.0, 9.0);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);

            FixedVector<Dim3> result = a - b;
            AssertVector(result, 4.0, 5.0, 6.0);
        }

        [Test]
        public void ScalarMultiplication_MultipliesEveryCoordinate()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense(1.0, -2.0, 3.0);
            FixedVector<Dim3> result = vector * 2.0;

            AssertVector(result, 2.0, -4.0, 6.0);
        }

        [Test]
        public void Dot_ReturnsScalarProduct()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(4.0, 5.0, 6.0);

            double result = a.Dot(b);
            Assert.That(result, Is.EqualTo(32.0).Within(Tolerance));
        }

        [Test]
        public void Dot_OfOrthogonalVectors_ReturnsZero()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(1.0, 0.0, 0.0);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(0.0, 1.0, 0.0);

            double result = a.Dot(b);
            
            Assert.That(result, Is.EqualTo(0.0).Within(Tolerance));
        }

        [Test]
        public void L2Norm_ReturnsEuclideanLength()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense(3.0, 4.0, 12.0);

            double result = vector.L2Norm();
            
            Assert.That(result, Is.EqualTo(13.0).Within(Tolerance));
        }

        [Test]
        public void Normalize_ReturnsUnitVector()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense(3.0, 0.0, 4.0);
            FixedVector<Dim3> normalized = vector.SafeNormalize();
            
            Assert.That(normalized.L2Norm(), Is.EqualTo(1.0).Within(Tolerance));
            AssertVector(normalized, 0.6, 0.0, 0.8);
        }

        [Test]
        public void Normalize_ZeroVector_FollowedDefinedContract()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Zeros();
            
            Assert.That(() => vector.Normalize(), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Indexer_NegativeIndex_Throws()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            Assert.Throws<ArgumentOutOfRangeException>(() => { double unused = vector[-1]; });
        }

        [Test]
        public void Indexer_IndexEqualToDimension_Throws()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            Assert.Throws<ArgumentOutOfRangeException>(() => { double unused = vector[3]; });
        }
        
        private static void AssertVector(FixedVector<Dim3> actual, double x, double y, double z)
        {
            Assert.That(actual[0], Is.EqualTo(x).Within(Tolerance));
            Assert.That(actual[1], Is.EqualTo(y).Within(Tolerance));
            Assert.That(actual[2], Is.EqualTo(z).Within(Tolerance));
        }
    }
}