using System;
using System.Collections.Generic;
using IFCConverter.Geometry;
using IFCConverter.Geometry.Mesh;
using IFCConverter.Utils.Mathematics;
using NUnit.Framework;

namespace IFCConverter.Tests.Geometry
{
    [TestFixture]
    public sealed class MeshTests
    {
        [Test]
        public void Constructor_EmptyCollections_CreatesEmptyMesh()
        {
            Mesh mesh = CreateMesh();

            Assert.That(mesh.VertexCount, Is.EqualTo(0));
            Assert.That(mesh.TriangleCount, Is.EqualTo(0));
            Assert.That(mesh.FaceNormalCount, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_WithoutNormals_CalculatesFaceNormals()
        {
            Mesh mesh = CreateTriangleMesh();

            Assert.That(mesh.FaceNormalCount, Is.EqualTo(1));
            AssertVectorEqual(Vector(0, 0, 1), mesh.FaceNormals[0]);
        }

        [Test]
        public void Constructor_WithNormals_UsesProvidedNormals()
        {
            FixedVector<Dim3> expectedNormal = Vector(1, 0, 0);

            Mesh mesh = new Mesh(
                TriangleVertices(),
                new[]
                {
                    new Triangle(0, 1, 2)
                },
                new[]
                {
                    expectedNormal
                });

            Assert.That(mesh.FaceNormalCount, Is.EqualTo(1));
            AssertVectorEqual(expectedNormal, mesh.FaceNormals[0]);
        }

        [Test]
        public void Constructor_WithEmptyNormals_DoesNotCalculateNormals()
        {
            Mesh mesh = new Mesh(
                TriangleVertices(),
                new[]
                {
                    new Triangle(0, 1, 2)
                },
                new FixedVector<Dim3>[0]);

            Assert.That(mesh.TriangleCount, Is.EqualTo(1));
            Assert.That(mesh.FaceNormalCount, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_CopiesInputCollections()
        {
            List<FixedVector<Dim3>> vertices = new List<FixedVector<Dim3>>(TriangleVertices());

            List<Triangle> triangles = new List<Triangle>
            {
                new Triangle(0, 1, 2)
            };

            Mesh mesh = new Mesh(vertices, triangles);

            vertices.Add(Vector(1, 1, 0));
            triangles.Clear();

            Assert.That(mesh.VertexCount, Is.EqualTo(3));
            Assert.That(mesh.TriangleCount, Is.EqualTo(1));
            Assert.That(mesh.FaceNormalCount, Is.EqualTo(1));
        }

        [Test]
        public void AddVertex_AddsVertexAndReturnsItsIndex()
        {
            Mesh mesh = CreateMesh();

            int index = mesh.AddVertex(Vector(1, 2, 3));

            Assert.That(index, Is.EqualTo(0));
            Assert.That(mesh.VertexCount, Is.EqualTo(1));
            AssertVectorEqual(Vector(1, 2, 3), mesh.Vertices[index]);
        }

        [Test]
        public void AddVertex_MultipleVertices_ReturnsSequentialIndices()
        {
            Mesh mesh = CreateMesh();

            int firstIndex = mesh.AddVertex(Vector(1, 0, 0));
            int secondIndex = mesh.AddVertex(Vector(2, 0, 0));

            Assert.That(firstIndex, Is.EqualTo(0));
            Assert.That(secondIndex, Is.EqualTo(1));
            Assert.That(mesh.VertexCount, Is.EqualTo(2));
        }

        [Test]
        public void AddTriangle_AddsTriangleAndReturnsItsIndex()
        {
            Mesh mesh = CreateMesh(Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0));

            int index = mesh.AddTriangle(new Triangle(0, 1, 2));

            Assert.That(index, Is.EqualTo(0));
            Assert.That(mesh.TriangleCount, Is.EqualTo(1));

            Triangle triangle = mesh.Triangles[index];

            Assert.That(triangle.A, Is.EqualTo(0));
            Assert.That(triangle.B, Is.EqualTo(1));
            Assert.That(triangle.C, Is.EqualTo(2));
        }

        [Test]
        public void AddTriangle_DoesNotAutomaticallyAddFaceNormal()
        {
            Mesh mesh = CreateMesh(Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0));
            mesh.AddTriangle(new Triangle(0, 1, 2));

            Assert.That(mesh.TriangleCount, Is.EqualTo(1));
            Assert.That(mesh.FaceNormalCount, Is.EqualTo(0));
        }

        [Test]
        public void AddFaceNormal_AddsNormalAndReturnsItsIndex()
        {
            Mesh mesh = CreateMesh();
            FixedVector<Dim3> expectedNormal = Vector(0, 0, 1);

            int index = mesh.AddFaceNormal(expectedNormal);

            Assert.That(index, Is.EqualTo(0));
            Assert.That(mesh.FaceNormalCount, Is.EqualTo(1));
            AssertVectorEqual(expectedNormal, mesh.FaceNormals[index]);
        }

        [Test]
        public void AddFaceNormal_MultipleNormals_ReturnsSequentialIndices()
        {
            Mesh mesh = CreateMesh();

            int firstIndex = mesh.AddFaceNormal(Vector(1, 0, 0));
            int secondIndex = mesh.AddFaceNormal(Vector(0, 1, 0));

            Assert.That(firstIndex, Is.EqualTo(0));
            Assert.That(secondIndex, Is.EqualTo(1));
            Assert.That(mesh.FaceNormalCount, Is.EqualTo(2));
        }

        private static Mesh CreateTriangleMesh()
        {
            return new Mesh(TriangleVertices(), new[]
            {
                new Triangle(0, 1, 2)
            });
        }

        private static Mesh CreateMesh(params FixedVector<Dim3>[] vertices)
        {
            return new Mesh(vertices, Array.Empty<Triangle>());
        }

        private static FixedVector<Dim3>[] TriangleVertices()
        {
            return new[]
            {
                Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0)
            };
        }

        private static FixedVector<Dim3> Vector(double x, double y, double z)
        {
            return FixedVector<Dim3>.Builder.Dense(x, y, z);
        }

        private static void AssertVectorEqual(FixedVector<Dim3> expected, FixedVector<Dim3> actual)
        {
            Assert.That(actual[0], Is.EqualTo(expected[0]).Within(1e-10));
            Assert.That(actual[1], Is.EqualTo(expected[1]).Within(1e-10));
            Assert.That(actual[2], Is.EqualTo(expected[2]).Within(1e-10));
        }
    }
}