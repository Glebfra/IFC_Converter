using System;
using System.Linq;
using IFCConverter.Geometry;
using NUnit.Framework;

namespace IFCConverter.Tests.Geometry
{
    [TestFixture]
    public sealed class TriangleTests
    {
        private static readonly Triangle Triangle = new Triangle(4, 2, 7);

        [Test]
        public void Constructor_PreservesVertexOrder()
        {
            Assert.That(Triangle.A, Is.EqualTo(4));
            Assert.That(Triangle.B, Is.EqualTo(2));
            Assert.That(Triangle.C, Is.EqualTo(7));
        }

        [Test]
        public void Indexer_ReturnsVerticesInOrder()
        {
            Assert.That(Triangle[0], Is.EqualTo(4));
            Assert.That(Triangle[1], Is.EqualTo(2));
            Assert.That(Triangle[2], Is.EqualTo(7));
        }

        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(100)]
        public void Indexer_ThrowsForInvalidIndex(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = Triangle[index]);
        }

        [Test]
        public void GetEnumerator_ReturnsVerticesInOrder()
        {
            Assert.That(Triangle.ToArray(), Is.EqualTo(new[]
            {
                4, 2, 7
            }));
        }

        [Test]
        public void GetEdges_ReturnsThreeEdges()
        {
            Edge[] edges = Triangle.GetEdges().ToArray();

            Assert.That(edges.Length, Is.EqualTo(3));
            Assert.That(edges, Does.Contain(new Edge(4, 2)));
            Assert.That(edges, Does.Contain(new Edge(2, 7)));
            Assert.That(edges, Does.Contain(new Edge(7, 4)));
        }

        [Test]
        public void EdgeAB_ReturnsEdgeBetweenAAndB()
        {
            Assert.That(Triangle.EdgeAB, Is.EqualTo(new Edge(4, 2)));
        }

        [Test]
        public void EdgeBC_ReturnsEdgeBetweenBAndC()
        {
            Assert.That(Triangle.EdgeBC, Is.EqualTo(new Edge(2, 7)));
        }

        [Test]
        public void EdgeCA_ReturnsEdgeBetweenCAndA()
        {
            Assert.That(Triangle.EdgeCA, Is.EqualTo(new Edge(7, 4)));
        }

        [Test]
        public void GetEdges_ReturnsUniqueEdgesForTriangleWithDistinctVertices()
        {
            Edge[] edges = Triangle.GetEdges().ToArray();

            Assert.That(edges.Distinct().Count(), Is.EqualTo(3));
        }

        [Test]
        public void Triangle_AllowsRepeatedVertexIndices()
        {
            Triangle triangle = new Triangle(1, 1, 2);

            Assert.That(triangle.A, Is.EqualTo(1));
            Assert.That(triangle.B, Is.EqualTo(1));
            Assert.That(triangle.C, Is.EqualTo(2));
        }
    }
}