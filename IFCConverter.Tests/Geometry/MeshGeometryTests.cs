using System;
using System.Collections.Generic;
using IFCConverter.Geometry;
using IFCConverter.Geometry.Algorithms;
using IFCConverter.Geometry.Mesh;
using IFCConverter.Utils.Mathematics;
using NUnit.Framework;

namespace IFCConverter.Tests.Geometry
{
    [TestFixture]
    public sealed class MeshGeometryTests
    {
        private const double Tolerance = 1e-10;
        
        [Test]
        public void GetFaceNormal_FromThreeVertices_ReturnsExpectedNormal()
        {
            FixedVector<Dim3> a = Vector(0, 0, 0);
            FixedVector<Dim3> b = Vector(1, 0, 0);
            FixedVector<Dim3> c = Vector(0, 1, 0);

            FixedVector<Dim3> result = MeshGeometry.GetFaceNormal(a, b, c);

            AssertVectorEqual(Vector(0, 0, 1), result);
        }

        [Test]
        public void GetFaceNormal_FromThreeVertices_ReturnsUnitVector()
        {
            FixedVector<Dim3> a = Vector(0, 0, 0);
            FixedVector<Dim3> b = Vector(2, 0, 0);
            FixedVector<Dim3> c = Vector(0, 3, 0);

            FixedVector<Dim3> result = MeshGeometry.GetFaceNormal(a, b, c);

            Assert.That(result.L2Norm(), Is.EqualTo(1.0).Within(Tolerance));
        }

        [Test]
        public void GetFaceNormal_FromThreeVertices_IsPerpendicularToTriangleEdges()
        {
            FixedVector<Dim3> a = Vector(1, 2, 3);
            FixedVector<Dim3> b = Vector(4, 2, 3);
            FixedVector<Dim3> c = Vector(1, 6, 3);

            FixedVector<Dim3> result = MeshGeometry.GetFaceNormal(a, b, c);

            Assert.That(result.Dot(b - a), Is.EqualTo(0.0).Within(Tolerance));
            Assert.That(result.Dot(c - a), Is.EqualTo(0.0).Within(Tolerance));
        }

        [Test]
        public void GetFaceNormal_ReversingVertexOrder_ReversesNormal()
        {
            FixedVector<Dim3> a = Vector(0, 0, 0);
            FixedVector<Dim3> b = Vector(1, 0, 0);
            FixedVector<Dim3> c = Vector(0, 1, 0);

            FixedVector<Dim3> forward = MeshGeometry.GetFaceNormal(a, b, c);
            FixedVector<Dim3> reversed = MeshGeometry.GetFaceNormal(a, c, b);

            AssertVectorEqual(Vector(0, 0, 1), forward);
            AssertVectorEqual(Vector(0, 0, -1), reversed);
        }

        [Test]
        public void GetFaceNormal_FromVertexList_ReturnsExpectedNormal()
        {
            IReadOnlyList<FixedVector<Dim3>> vertices = new[]
            {
                Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0)
            };

            FixedVector<Dim3> result = MeshGeometry.GetFaceNormal(vertices);

            AssertVectorEqual(Vector(0, 0, 1), result);
        }

        [Test]
        public void GetFaceNormal_FromVertexList_WithTooFewVertices_ThrowsArgumentException()
        {
            IReadOnlyList<FixedVector<Dim3>> vertices = new[]
            {
                Vector(0, 0, 0), Vector(1, 0, 0)
            };

            Assert.Throws<ArgumentException>(() => MeshGeometry.GetFaceNormal(vertices));
        }

        [Test]
        public void GetFaceNormal_FromVertexList_WithTooManyVertices_ThrowsArgumentException()
        {
            IReadOnlyList<FixedVector<Dim3>> vertices = new[]
            {
                Vector(0, 0, 0), Vector(1, 0, 0), Vector(0, 1, 0), Vector(1, 1, 0)
            };

            Assert.Throws<ArgumentException>(() => MeshGeometry.GetFaceNormal(vertices));
        }

        [Test]
        public void GetFaceNormal_FromMesh_ReturnsExpectedNormal()
        {
            Mesh mesh = CreateMesh(new Triangle(0, 1, 2));

            FixedVector<Dim3> result = MeshGeometry.GetFaceNormal(mesh, 0);

            AssertVectorEqual(Vector(0, 0, 1), result);
        }

        [Test]
        public void GetFaceNormal_FromMesh_WithInvalidTriangleIndex_ThrowsArgumentOutOfRangeException()
        {
            Mesh mesh = CreateMesh(new Triangle(0, 1, 2));

            Assert.Throws<ArgumentOutOfRangeException>(() => MeshGeometry.GetFaceNormal(mesh, 1));
        }

        [Test]
        public void GetFaceNormal_FromMesh_WithNegativeTriangleIndex_ThrowsArgumentOutOfRangeException()
        {
            Mesh mesh = CreateMesh(new Triangle(0, 1, 2));

            Assert.Throws<ArgumentOutOfRangeException>(() => MeshGeometry.GetFaceNormal(mesh, -1));
        }

        [Test]
        public void GetFaceNormal_WithCollinearVertices_ThrowsInvalidOperationException()
        {
            FixedVector<Dim3> a = Vector(0, 0, 0);
            FixedVector<Dim3> b = Vector(1, 1, 1);
            FixedVector<Dim3> c = Vector(2, 2, 2);
            
            AssertVectorEqual(MeshGeometry.GetFaceNormal(a, b, c), FixedVector<Dim3>.Zeros());
        }

        [Test]
        public void GetFaceNormal_WithCoincidentVertices_ThrowsInvalidOperationException()
        {
            FixedVector<Dim3> a = Vector(1, 1, 1);
            FixedVector<Dim3> b = Vector(1, 1, 1);
            FixedVector<Dim3> c = Vector(2, 2, 2);
            
            AssertVectorEqual(MeshGeometry.GetFaceNormal(a, b, c), FixedVector<Dim3>.Zeros());
        }

        private static Mesh CreateMesh(params Triangle[] triangles)
        {
            FixedVector<Dim3>[] vertices =
            {
                Vector(0, 0, 0),
                Vector(1, 0, 0),
                Vector(0, 1, 0)
            };

            return new Mesh(vertices, triangles);
        }

        private static FixedVector<Dim3> Vector(double x, double y, double z)
        {
            return FixedVector<Dim3>.Builder.Dense(x, y, z);
        }

        private static void AssertVectorEqual(FixedVector<Dim3> expected, FixedVector<Dim3> actual)
        {
            Assert.That(actual[0], Is.EqualTo(expected[0]).Within(Tolerance));
            Assert.That(actual[1], Is.EqualTo(expected[1]).Within(Tolerance));
            Assert.That(actual[2], Is.EqualTo(expected[2]).Within(Tolerance));
        }
    }
}