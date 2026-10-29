using System;
using IFCConverter.Utils.Mathematics;
using NUnit.Framework;

namespace IFCConverter.Tests.Mathematics
{
    [TestFixture]
    public sealed class FixedMatrixTests
    {
        private const double Tolerance = 1e-10;

        private static FixedMatrix<Dim2> CreateMatrix2x2()
        {
            return FixedMatrix<Dim2>.Builder.Dense(new[,]
            {
                {
                    1.0, 2.0
                },
                {
                    3.0, 4.0
                }
            });
        }

        private static void AssertMatrixEqual<TDimension>(FixedMatrix<TDimension> actual, double[,] expected)
            where TDimension : struct, IDimension
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(expected.GetLength(0), Is.EqualTo(FixedMatrix<TDimension>.Dimension));
            Assert.That(expected.GetLength(1), Is.EqualTo(FixedMatrix<TDimension>.Dimension));

            for (int row = 0; row < FixedMatrix<TDimension>.Dimension; row++)
            {
                for (int col = 0; col < FixedMatrix<TDimension>.Dimension; col++)
                {
                    Assert.That(actual[row, col], Is.EqualTo(expected[row, col]).Within(Tolerance), $"Unexpected value at [{row}, {col}].");
                }
            }
        }

        [Test]
        public void Multiply_MatrixByVector_ReturnsCorrectVector()
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

            FixedVector<Dim2> vector = FixedVector<Dim2>.Builder.Dense(5, 6);

            FixedVector<Dim2> result = matrix * vector;

            Assert.That(result[0], Is.EqualTo(17).Within(1e-10));
            Assert.That(result[1], Is.EqualTo(39).Within(1e-10));
        }

        [Test]
        public void LeftMultiply_VectorByMatrix_ReturnsCorrectVector()
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

            FixedVector<Dim2> vector = FixedVector<Dim2>.Builder.Dense(5, 6);

            FixedVector<Dim2> result = vector * matrix;

            Assert.That(result[0], Is.EqualTo(23).Within(1e-10));
            Assert.That(result[1], Is.EqualTo(34).Within(1e-10));
        }

        [Test]
        public void SetColumn_ReplacesColumnValues()
        {
            FixedMatrix<Dim3> matrix = FixedMatrix<Dim3>.Builder.Dense();
            FixedVector<Dim3> vector = FixedVector<Dim3>.Builder.Dense(1, 2, 3);

            matrix.SetColumn(1, vector);

            Assert.That(matrix[0, 1], Is.EqualTo(1));
            Assert.That(matrix[1, 1], Is.EqualTo(2));
            Assert.That(matrix[2, 1], Is.EqualTo(3));
        }

        [Test]
        public void GetColumn_ReturnsColumnValues()
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

            Assert.That(column[0], Is.EqualTo(2));
            Assert.That(column[1], Is.EqualTo(5));
            Assert.That(column[2], Is.EqualTo(8));
        }

        [Test]
        public void SetSubMatrix_CanWriteIntoBottomRightCorner()
        {
            FixedMatrix<Dim3> matrix = FixedMatrix<Dim3>.Builder.Dense();
            FixedMatrix<Dim2> subMatrix = FixedMatrix<Dim2>.Builder.Dense(new[,]
            {
                {
                    1.0, 2.0
                },
                {
                    3.0, 4.0
                }
            });

            matrix.SetSubMatrix(subMatrix, 1, 1);

            Assert.That(matrix[1, 1], Is.EqualTo(1));
            Assert.That(matrix[1, 2], Is.EqualTo(2));
            Assert.That(matrix[2, 1], Is.EqualTo(3));
            Assert.That(matrix[2, 2], Is.EqualTo(4));
        }

        [Test]
        public void BuilderDense_WithoutValues_CreatesZeroMatrix()
        {
            FixedMatrix<Dim2> matrix = FixedMatrix<Dim2>.Builder.Dense();

            AssertMatrixEqual(matrix, new[,]
            {
                {
                    0.0, 0.0
                },
                {
                    0.0, 0.0
                }
            });
        }

        [Test]
        public void BuilderDense_WithArray_CreatesMatrix()
        {
            FixedMatrix<Dim2> matrix = CreateMatrix2x2();

            AssertMatrixEqual(matrix, new[,]
            {
                {
                    1.0, 2.0
                },
                {
                    3.0, 4.0
                }
            });
        }

        [Test]
        public void BuilderDense_WithIncorrectNumberOfValues_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => FixedMatrix<Dim2>.Builder.Dense(1.0, 2.0, 3.0));
        }

        [Test]
        public void BuilderDense_WithIncorrectArrayDimensions_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => FixedMatrix<Dim2>.Builder.Dense(new[,]
            {
                {
                    1.0, 2.0, 3.0
                },
                {
                    4.0, 5.0, 6.0
                }
            }));
        }

        [Test]
        public void Indexer_CanReadAndWriteValues()
        {
            FixedMatrix<Dim2> matrix = FixedMatrix<Dim2>.Builder.Dense();
            matrix[0, 1] = 12.5;
            Assert.That(matrix[0, 1], Is.EqualTo(12.5));
        }

        [Test]
        public void Add_ReturnsElementwiseSum()
        {
            FixedMatrix<Dim2> a = CreateMatrix2x2();
            FixedMatrix<Dim2> b = FixedMatrix<Dim2>.Builder.Dense(new[,]
            {
                {
                    5.0, 6.0
                },
                {
                    7.0, 8.0
                }
            });

            AssertMatrixEqual(a + b, new[,]
            {
                {
                    6.0, 8.0
                },
                {
                    10.0, 12.0
                }
            });
        }

        [Test]
        public void Subtract_ReturnsElementwiseDifference()
        {
            FixedMatrix<Dim2> a = FixedMatrix<Dim2>.Builder.Dense(new[,]
            {
                {
                    5.0, 6.0
                },
                {
                    7.0, 8.0
                }
            });
            FixedMatrix<Dim2> b = CreateMatrix2x2();

            AssertMatrixEqual(a - b, new[,]
            {
                {
                    4.0, 4.0
                },
                {
                    4.0, 4.0
                }
            });
        }

        [Test]
        public void MultiplyMatrix_ReturnsMatrixProduct()
        {
            FixedMatrix<Dim2> a = CreateMatrix2x2();
            FixedMatrix<Dim2> b = FixedMatrix<Dim2>.Builder.Dense(new[,]
            {
                {
                    5.0, 6.0
                },
                {
                    7.0, 8.0
                }
            });

            AssertMatrixEqual(a * b, new[,]
            {
                {
                    19.0, 22.0
                },
                {
                    43.0, 50.0
                }
            });
        }

        [Test]
        public void Transpose_SwapsRowsAndColumns()
        {
            FixedMatrix<Dim2> matrix = CreateMatrix2x2();

            AssertMatrixEqual(matrix.Transpose(), new[,]
            {
                {
                    1.0, 3.0
                },
                {
                    2.0, 4.0
                }
            });
        }

        [Test]
        public void Identity_ReturnsIdentityMatrix()
        {
            AssertMatrixEqual(FixedMatrix<Dim2>.Identity(), new[,]
            {
                {
                    1.0, 0.0
                },
                {
                    0.0, 1.0
                }
            });
        }

        [Test]
        public void Ones_ReturnsMatrixFilledWithOnes()
        {
            AssertMatrixEqual(FixedMatrix<Dim2>.Ones(), new[,]
            {
                {
                    1.0, 1.0
                },
                {
                    1.0, 1.0
                }
            });
        }

        [Test]
        public void Zeros_ReturnsZeroMatrix()
        {
            AssertMatrixEqual(FixedMatrix<Dim2>.Zeros(), new[,]
            {
                {
                    0.0, 0.0
                },
                {
                    0.0, 0.0
                }
            });
        }

        [Test]
        public void Determinant_ReturnsCorrectValue()
        {
            FixedMatrix<Dim2> matrix = CreateMatrix2x2();
            Assert.That(matrix.Determinant(), Is.EqualTo(-2.0).Within(Tolerance));
        }

        [Test]
        public void Inverse_ReturnsInverseMatrix()
        {
            FixedMatrix<Dim2> matrix = CreateMatrix2x2();

            AssertMatrixEqual(matrix.Inverse(), new[,]
            {
                {
                    -2.0, 1.0
                },
                {
                    1.5, -0.5
                }
            });
        }

        [Test]
        public void Inverse_MultipliedByOriginal_ReturnsIdentity()
        {
            FixedMatrix<Dim2> matrix = CreateMatrix2x2();

            AssertMatrixEqual(matrix * matrix.Inverse(), new[,]
            {
                {
                    1.0, 0.0
                },
                {
                    0.0, 1.0
                }
            });
        }

        [Test]
        public void Clone_CopiesValues()
        {
            FixedMatrix<Dim2> original = CreateMatrix2x2();
            FixedMatrix<Dim2> clone = original.Clone();

            AssertMatrixEqual(clone, new[,]
            {
                {
                    1.0, 2.0
                },
                {
                    3.0, 4.0
                }
            });
        }

        [Test]
        public void Clone_ModifyingClone_DoesNotModifyOriginal()
        {
            FixedMatrix<Dim2> original = CreateMatrix2x2();
            FixedMatrix<Dim2> clone = original.Clone();

            clone[0, 0] = 100.0;

            Assert.That(original[0, 0], Is.EqualTo(1.0));
            Assert.That(clone[0, 0], Is.EqualTo(100.0));
        }

        [Test]
        public void Identity_MultipliedByMatrix_ReturnsOriginalMatrix()
        {
            FixedMatrix<Dim2> matrix = CreateMatrix2x2();

            AssertMatrixEqual(FixedMatrix<Dim2>.Identity() * matrix, new[,]
            {
                {
                    1.0, 2.0
                },
                {
                    3.0, 4.0
                }
            });
        }

        [Test]
        public void Matrix_MultipliedByIdentity_ReturnsOriginalMatrix()
        {
            FixedMatrix<Dim2> matrix = CreateMatrix2x2();

            AssertMatrixEqual(matrix * FixedMatrix<Dim2>.Identity(), new[,]
            {
                {
                    1.0, 2.0
                },
                {
                    3.0, 4.0
                }
            });
        }
    }
}