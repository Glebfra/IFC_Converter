using System.Collections.Generic;
using IFCConverter.Geometry;
using IFCConverter.Geometry.Algorithms;
using IFCConverter.Geometry.Mesh;
using IFCConverter.Utils.Mathematics;
using NUnit.Framework;

namespace IFCConverter.Tests.Geometry
{
    [TestFixture]
    public sealed class MeshTopologyTests
    {
        [Test]
        public void GetEdgeUsage_EmptyMesh_ReturnsEmptyDictionary()
        {
            Mesh mesh = CreateMesh();

            IDictionary<Edge, int> result = MeshTopology.GetEdgeUsage(mesh);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetEdgeUsage_SingleTriangle_EachEdgeUsedOnce()
        {
            Mesh mesh = CreateMesh(new Triangle(0, 1, 2));

            IDictionary<Edge, int> result = MeshTopology.GetEdgeUsage(mesh);

            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[new Edge(0, 1)], Is.EqualTo(1));
            Assert.That(result[new Edge(1, 2)], Is.EqualTo(1));
            Assert.That(result[new Edge(0, 2)], Is.EqualTo(1));
        }

        [Test]
        public void GetEdgeUsage_TwoTriangles_SharedEdgeUsedTwice()
        {
            Mesh mesh = CreateMesh(new Triangle(0, 1, 2), new Triangle(2, 1, 3));

            IDictionary<Edge, int> result = MeshTopology.GetEdgeUsage(mesh);

            Assert.That(result, Has.Count.EqualTo(5));
            Assert.That(result[new Edge(1, 2)], Is.EqualTo(2));

            Assert.That(result[new Edge(0, 1)], Is.EqualTo(1));
            Assert.That(result[new Edge(0, 2)], Is.EqualTo(1));
            Assert.That(result[new Edge(1, 3)], Is.EqualTo(1));
            Assert.That(result[new Edge(2, 3)], Is.EqualTo(1));
        }

        [Test]
        public void GetEdgeUsage_SharedEdgeHasReversedDirection_CountsAsSameEdge()
        {
            Mesh mesh = CreateMesh(new Triangle(0, 1, 2), new Triangle(3, 2, 1));

            IDictionary<Edge, int> result = MeshTopology.GetEdgeUsage(mesh);

            Assert.That(result[new Edge(1, 2)], Is.EqualTo(2));
        }

        [Test]
        public void GetBoundaryEdges_EmptyMesh_ReturnsEmptySequence()
        {
            Mesh mesh = CreateMesh();

            IEnumerable<Edge> result = MeshTopology.GetBoundaryEdges(mesh);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetBoundaryEdges_SingleTriangle_ReturnsAllThreeEdges()
        {
            Mesh mesh = CreateMesh(
                new Triangle(0, 1, 2));

            IEnumerable<Edge> result = MeshTopology.GetBoundaryEdges(mesh);

            CollectionAssert.AreEquivalent(new[]
                {
                    new Edge(0, 1), new Edge(1, 2), new Edge(0, 2)
                },
                result);
        }

        [Test]
        public void GetBoundaryEdges_TwoTriangles_ExcludesSharedEdge()
        {
            Mesh mesh = CreateMesh(new Triangle(0, 1, 2), new Triangle(2, 1, 3));

            IEnumerable<Edge> result = MeshTopology.GetBoundaryEdges(mesh);

            CollectionAssert.AreEquivalent(new[]
                {
                    new Edge(0, 1), new Edge(0, 2), new Edge(1, 3), new Edge(2, 3)
                },
                result);
        }

        [Test]
        public void GetBoundaryVertices_EmptyMesh_ReturnsEmptySequence()
        {
            Mesh mesh = CreateMesh();

            IEnumerable<int> result = MeshTopology.GetBoundaryVertices(mesh);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetBoundaryVertices_SingleTriangle_ReturnsAllThreeVertices()
        {
            Mesh mesh = CreateMesh(new Triangle(0, 1, 2));

            IEnumerable<int> result = MeshTopology.GetBoundaryVertices(mesh);

            CollectionAssert.AreEquivalent(new[] { 0, 1, 2 }, result);
        }

        [Test]
        public void GetBoundaryVertices_TwoTriangles_ReturnsUniqueVertices()
        {
            Mesh mesh = CreateMesh(new Triangle(0, 1, 2), new Triangle(2, 1, 3));

            IEnumerable<int> result = MeshTopology.GetBoundaryVertices(mesh);

            CollectionAssert.AreEquivalent(new[] { 0, 1, 2, 3 }, result);
        }

        private static Mesh CreateMesh(params Triangle[] triangles)
        {
            FixedVector<Dim3>[] vertices =
            {
                FixedVector<Dim3>.Builder.Dense(0, 0, 0), FixedVector<Dim3>.Builder.Dense(1, 0, 0), FixedVector<Dim3>.Builder.Dense(0, 1, 0),
                FixedVector<Dim3>.Builder.Dense(1, 1, 0)
            };

            return new Mesh(vertices, triangles);
        }
    }
}