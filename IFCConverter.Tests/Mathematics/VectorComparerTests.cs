using IFCConverter.Utils.Mathematics;
using NUnit.Framework;

namespace IFCConverter.Tests.Mathematics
{
    [TestFixture]
    public sealed class VectorComparerTests
    {
        private const double Tolerance = 1e-3;
        private VectorComparer _comparer;

        [SetUp]
        public void SetUp()
        {
            _comparer = new VectorComparer(Tolerance);
        }

        [Test]
        public void Equals_IdenticalVectors_ReturnsTrue()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            
            Assert.That(_comparer.Equals(a, b), Is.True);
        }

        [Test]
        public void Equals_DifferenceBelowTolerance_ReturnsTrue()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(1.0 + 0.0005, 2.0, 3.0);
            
            Assert.That(_comparer.Equals(a, b), Is.True);
        }

        [Test]
        public void Equals_DifferenceAboveTolerance_ReturnsFalse()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(1.0 + 0.002, 2.0, 3.0);
            
            Assert.That(_comparer.Equals(a, b), Is.False);
        }

        [Test]
        public void Equals_DifferenceInAnyCoordinate_IsChecked()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0 + 0.002);

            Assert.That(_comparer.Equals(a, b), Is.False);
        }

        [Test]
        public void Equals_IsSymmetric()
        {
            FixedVector<Dim3> a = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);
            FixedVector<Dim3> b = FixedVector<Dim3>.Builder.Dense(1.0 + 0.0005, 2.0, 3.0);
            
            Assert.That(_comparer.Equals(a, b), Is.EqualTo(_comparer.Equals(b, a)));
        }
    }
}