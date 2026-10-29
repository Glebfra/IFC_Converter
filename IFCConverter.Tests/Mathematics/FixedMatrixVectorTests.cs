using IFCConverter.Utils.Mathematics;
using NUnit.Framework;

namespace IFCConverter.Tests.Mathematics
{
    [TestFixture]
    public sealed class FixedMatrixVectorTests
    {
        private const double Tolerance = 1e-10;

        [Test]
        public void Multiply_MatrixByColumnVector_ReturnsCorrectVector()
        {
            FixedMatrix<Dim2> matrix = FixedMatrix<Dim2>.Builder.Dense(new[,]
            {
                {
                    1.0, 2.0
                },
                {
                    3.0, 4.0
                }
            });
            FixedVector<Dim2> vector = FixedVector<Dim2>.Builder.Dense(5.0, 6.0);

            FixedVector<Dim2> result = matrix.Multiply(vector);

            Assert.That(result[0], Is.EqualTo(17.0).Within(Tolerance));
            Assert.That(result[1], Is.EqualTo(39.0).Within(Tolerance));
        }

        [Test]
        public void LeftMultiply_RowVectorByMatrix_ReturnsCorrectVector()
        {
            FixedMatrix<Dim2> matrix = FixedMatrix<Dim2>.Builder.Dense(new[,]
            {
                {
                    1.0, 2.0
                },
                {
                    3.0, 4.0
                }
            });
            FixedVector<Dim2> vector = FixedVector<Dim2>.Builder.Dense(5.0, 6.0);

            FixedVector<Dim2> result = matrix.LeftMultiply(vector);

            Assert.That(result[0], Is.EqualTo(23.0).Within(Tolerance));
            Assert.That(result[1], Is.EqualTo(34.0).Within(Tolerance));
        }

        [Test]
        public void GetColumn_ReturnsRequestedColumn()
        {
            FixedMatrix<Dim3> matrix = FixedMatrix<Dim3>.Builder.Dense(new[,]
            {
                {
                    1.0, 2.0, 3.0
                },
                {
                    4.0, 5.0, 6.0
                },
                {
                    7.0, 8.0, 9.0
                }
            });
            FixedVector<Dim3> column = matrix.GetColumn(1);

            Assert.That(column[0], Is.EqualTo(2.0));
            Assert.That(column[1], Is.EqualTo(5.0));
            Assert.That(column[2], Is.EqualTo(8.0));
        }

        [Test]
        public void SetColumn_ReplacesRequestedColumn()
        {
            FixedMatrix<Dim3> matrix = FixedMatrix<Dim3>.Builder.Dense();
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense(1.0, 2.0, 3.0);

            matrix.SetColumn(1, vector);

            Assert.That(matrix[0, 1], Is.EqualTo(1.0));
            Assert.That(matrix[1, 1], Is.EqualTo(2.0));
            Assert.That(matrix[2, 1], Is.EqualTo(3.0));
        }
    }
}