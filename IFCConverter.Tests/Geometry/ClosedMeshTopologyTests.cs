using System;
using System.Collections.Generic;
using IFCConverter.Geometry;
using IFCConverter.Geometry.Algorithms;
using IFCConverter.Utils.Mathematics;
using NUnit.Framework;

namespace IFCConverter.Tests.Geometry
{
    [TestFixture]
    public sealed class ClosedMeshTopologyTests
    {
        [Test]
        public void GetBoundaryEdges_EmptyMesh_ReturnsEmptySequence()
        {
            Mesh mesh = CreateMesh(Array.Empty<FixedVector<Dim3>>(), Array.Empty<Triangle>());

            IEnumerable<Edge> result = ClosedMeshTopology.GetBoundaryEdges(mesh);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetBoundaryEdges_OneTriangle_ReturnsEmptySequence()
        {
            Mesh mesh = CreateMesh(
                new[]
                {
                    Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0)
                },
                new[]
                {
                    new Triangle(0, 1, 2)
                });

            IEnumerable<Edge> result = ClosedMeshTopology.GetBoundaryEdges(mesh);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetBoundaryEdges_TwoCoplanarTriangles_ReturnsEmptySequence()
        {
            Mesh mesh = CreateSquareMesh();

            IEnumerable<Edge> result = ClosedMeshTopology.GetBoundaryEdges(mesh);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetBoundaryEdges_ThreeCoplanarTriangles_ReturnsBoundaryOfSelectedTriangles()
        {
            FixedVector<Dim3>[] vertices =
            {
                Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0), Vector(1, 1, 0), Vector(2, 0, 0)
            };

            Triangle[] triangles =
            {
                new Triangle(0, 1, 2), new Triangle(1, 3, 2), new Triangle(1, 4, 3)
            };

            Mesh mesh = CreateMesh(vertices, triangles);

            IEnumerable<Edge> result = ClosedMeshTopology.GetBoundaryEdges(mesh);

            Edge[] expected =
            {
                new Edge(0, 1), new Edge(0, 2), new Edge(1, 4), new Edge(3, 4), new Edge(2, 3)
            };

            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void GetBoundaryEdges_SeparateNormalGroups_SelectsOnlyGroupsWithMoreThanTwoTriangles()
        {
            FixedVector<Dim3>[] vertices =
            {
                // Три треугольника в плоскости XY.
                Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0), Vector(1, 1, 0), Vector(2, 0, 0),

                // Два треугольника в плоскости XZ.
                Vector(10, 0, 0), Vector(11, 0, 0), Vector(10, 0, 1), Vector(11, 0, 1)
            };

            Triangle[] triangles =
            {
                new Triangle(0, 1, 2), new Triangle(1, 3, 2), new Triangle(1, 4, 3), new Triangle(5, 6, 7), new Triangle(6, 8, 7)
            };

            Mesh mesh = CreateMesh(vertices, triangles);

            IEnumerable<Edge> result = ClosedMeshTopology.GetBoundaryEdges(mesh);

            Assert.That(result, Is.Not.Empty);

            Assert.That(result, Does.Contain(new Edge(0, 1)));
            Assert.That(result, Does.Contain(new Edge(1, 4)));

            Assert.That(result, Does.Not.Contains(new Edge(5, 6)));
            Assert.That(result, Does.Not.Contains(new Edge(6, 8)));
        }

        [Test]
        public void GetBoundaryEdges_ClosedTetrahedron_ReturnsEmptySequence()
        {
            FixedVector<Dim3>[] vertices =
            {
                Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0), Vector(0, 0, 1)
            };

            Triangle[] triangles =
            {
                new Triangle(0, 2, 1), new Triangle(0, 1, 3), new Triangle(1, 2, 3), new Triangle(2, 0, 3)
            };

            Mesh mesh = CreateMesh(vertices, triangles);

            IEnumerable<Edge> result = ClosedMeshTopology.GetBoundaryEdges(mesh);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetBoundaryEdges_UsesSuppliedFaceNormals()
        {
            FixedVector<Dim3>[] vertices =
            {
                Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0), Vector(1, 1, 0), Vector(2, 0, 0)
            };

            Triangle[] triangles =
            {
                new Triangle(0, 1, 2), new Triangle(1, 3, 2), new Triangle(1, 4, 3)
            };

            FixedVector<Dim3>[] normals =
            {
                Vector(0, 0, 1), Vector(0, 0, 1), Vector(0, 0, 1)
            };

            Mesh mesh = CreateMesh(vertices, triangles, normals);

            IEnumerable<Edge> result = ClosedMeshTopology.GetBoundaryEdges(mesh);

            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Does.Contain(new Edge(0, 1)));
        }

        private static Mesh CreateSquareMesh()
        {
            return CreateMesh(
                new[]
                {
                    Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0), Vector(1, 1, 0)
                },
                new[]
                {
                    new Triangle(0, 1, 2), new Triangle(1, 3, 2)
                });
        }

        private static Mesh CreateMesh(IEnumerable<FixedVector<Dim3>> vertices, IEnumerable<Triangle> triangles, IEnumerable<FixedVector<Dim3>> normals = null)
        {
            return new Mesh(vertices, triangles, normals);
        }

        private static FixedVector<Dim3> Vector(double x, double y, double z)
        {
            return FixedVector<Dim3>.Builder.Dense(x, y, z);
        }
    }
}