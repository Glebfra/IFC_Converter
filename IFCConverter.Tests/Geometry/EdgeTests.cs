using System.Collections.Generic;
using IFCConverter.Geometry;
using NUnit.Framework;

namespace IFCConverter.Tests.Geometry
{
    [TestFixture]
    public sealed class EdgeTests
    {
        [Test]
        public void Constructor_OrdersVerticesByIndex()
        {
            Edge edge = new Edge(5, 2);

            Assert.That(edge.A, Is.EqualTo(2));
            Assert.That(edge.B, Is.EqualTo(5));
        }

        [Test]
        public void Constructor_PreservesAlreadyOrderedVertices()
        {
            Edge edge = new Edge(2, 5);

            Assert.That(edge.A, Is.EqualTo(2));
            Assert.That(edge.B, Is.EqualTo(5));
        }

        [Test]
        public void Constructor_AllowsSameVertexIndex()
        {
            Edge edge = new Edge(3, 3);

            Assert.That(edge.A, Is.EqualTo(3));
            Assert.That(edge.B, Is.EqualTo(3));
        }

        [Test]
        public void Equals_ReturnsTrueForSameVertices()
        {
            Edge first = new Edge(2, 5);
            Edge second = new Edge(2, 5);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void Equals_ReturnsTrueWhenVerticesAreReversed()
        {
            Edge first = new Edge(2, 5);
            Edge second = new Edge(5, 2);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void Equals_ReturnsFalseForDifferentVertices()
        {
            Edge first = new Edge(2, 5);
            Edge second = new Edge(2, 6);

            Assert.That(first.Equals(second), Is.False);
        }

        [Test]
        public void ObjectEquals_ReturnsTrueForEqualEdge()
        {
            object first = new Edge(2, 5);
            object second = new Edge(5, 2);

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void ObjectEquals_ReturnsFalseForNull()
        {
            Edge edge = new Edge(2, 5);

            Assert.That(edge.Equals((object)null), Is.False);
        }

        [Test]
        public void GetHashCode_IsEqualForReversedVertices()
        {
            Edge first = new Edge(2, 5);
            Edge second = new Edge(5, 2);

            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
        }

        [Test]
        public void HashSet_DoesNotStoreDuplicateUndirectedEdges()
        {
            HashSet<Edge> edges = new HashSet<Edge>
            {
                new Edge(2, 5),
                new Edge(5, 2),
                new Edge(2, 6)
            };

            Assert.That(edges.Count, Is.EqualTo(2));
        }

        [Test]
        public void ToString_ReturnsOrderedVertexIndices()
        {
            Edge edge = new Edge(5, 2);

            Assert.That(edge.ToString(), Is.EqualTo("(2, 5)"));
        }
    }
}