using System;
using System.Linq;
using IFCConverter.Utils.Mathematics;
using NUnit.Framework;

namespace IFCConverter.Tests.Mathematics
{
    [TestFixture]
    public sealed class FixedVectorExtensionsTests
    {
        private const double Tolerance = 1e-10;

        private static void AssertVectorEqual<TDimension>(
            FixedVector<TDimension> actual,
            params double[] expected)
            where TDimension : struct, IDimension
        {
            Assert.That(actual, Is.Not.Null);
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
        public void Sum_ReturnsSumOfVectors()
        {
            FixedVector<Dim3>[] vectors = new[]
            {
                FixedVector<Dim3>.Builder.Dense(1, 2, 3),
                FixedVector<Dim3>.Builder.Dense(4, 5, 6),
                FixedVector<Dim3>.Builder.Dense(7, 8, 9)
            };

            AssertVectorEqual(vectors.Sum(), 12, 15, 18);
        }

        [Test]
        public void Sum_EmptyCollection_ReturnsZeroVector()
        {
            FixedVector<Dim3>[] vectors = Array.Empty<FixedVector<Dim3>>();

            AssertVectorEqual(vectors.Sum(), 0, 0, 0);
        }

        [Test]
        public void Mean_ReturnsAverageVector()
        {
            FixedVector<Dim3>[] vectors = new[]
            {
                FixedVector<Dim3>.Builder.Dense(2, 4, 6),
                FixedVector<Dim3>.Builder.Dense(4, 6, 8)
            };

            AssertVectorEqual(vectors.Mean(), 3, 5, 7);
        }

        [Test]
        public void CrossProduct_XAxisAndYAxis_ReturnsZAxis()
        {
            FixedVector<Dim3> x = FixedVector<Dim3>.Builder.X();
            FixedVector<Dim3> y = FixedVector<Dim3>.Builder.Y();

            AssertVectorEqual(x.CrossProduct(y), 0, 0, 1);
        }

        [Test]
        public void CrossProduct_IsPerpendicularToBothInputs()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(1, 2, 3);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(4, 5, 6);

            FixedVector<Dim3> cross = a.CrossProduct(b);

            Assert.That(cross.Dot(a), Is.EqualTo(0).Within(Tolerance));
            Assert.That(cross.Dot(b), Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void CrossProduct_ParallelVectors_ReturnsZeroVector()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(1, 2, 3);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(2, 4, 6);

            AssertVectorEqual(a.CrossProduct(b), 0, 0, 0);
        }

        [Test]
        public void CreateNormalVector_ReturnsPerpendicularVector()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense(1, 2, 3);

            FixedVector<Dim3> normal = vector.CreateNormalVector();

            Assert.That(vector.IsNormal(normal), Is.True);
        }

        [Test]
        public void CreateNormalVector_ForZAxis_ReturnsPerpendicularVector()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Z();

            FixedVector<Dim3> normal = vector.CreateNormalVector();

            Assert.That(vector.IsNormal(normal), Is.True);
            Assert.That(normal.L2Norm(), Is.GreaterThan(0));
        }

        [Test]
        public void ToHomogeneous_AppendsSpecifiedComponent()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense(1, 2, 3);

            AssertVectorEqual(vector.ToHomogeneous(1), 1, 2, 3, 1);
            AssertVectorEqual(vector.ToHomogeneous(0), 1, 2, 3, 0);
        }

        [Test]
        public void ToCartesian_DividesCoordinatesByW()
        {
            FixedVector<Dim4> vector = FixedVector<Dim4>.Builder.Dense(2, 4, 6, 2);

            AssertVectorEqual(vector.ToCartesian(), 1, 2, 3);
        }

        [Test]
        public void ToCartesian_WhenWIsZero_UsesUnscaledCoordinates()
        {
            FixedVector<Dim4> vector = FixedVector<Dim4>.Builder.Dense(2, 4, 6, 0);

            AssertVectorEqual(vector.ToCartesian(), 2, 4, 6);
        }

        [Test]
        public void AxisBuilders_ReturnUnitVectors()
        {
            AssertVectorEqual(FixedVector<Dim2>.Builder.X(), 1, 0);
            AssertVectorEqual(FixedVector<Dim2>.Builder.Y(), 0, 1);

            AssertVectorEqual(FixedVector<Dim3>.Builder.X(), 1, 0, 0);
            AssertVectorEqual(FixedVector<Dim3>.Builder.Y(), 0, 1, 0);
            AssertVectorEqual(FixedVector<Dim3>.Builder.Z(), 0, 0, 1);

            AssertVectorEqual(FixedVector<Dim4>.Builder.W(), 0, 0, 0, 1);
        }

        [Test]
        public void SetX_SetY_SetZ_UpdateComponents()
        {
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense();

            vector.SetX(1);
            vector.SetY(2);
            vector.SetZ(3);

            AssertVectorEqual(vector, 1, 2, 3);
        }
    }
}