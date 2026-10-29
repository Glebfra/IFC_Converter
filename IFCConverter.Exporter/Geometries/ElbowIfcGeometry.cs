using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.Geometry;
using IFCConverter.Geometry.Mesh;
using IFCConverter.Geometry.Mesh.Builders;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Attributes;
using IFCConverter.IFC.Builders.Geometry.Tessellated;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.IFC.Interfaces.Geometry.Tessellated;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Exporter.Geometries
{
    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    internal sealed class ElbowIfcGeometry : IfcGeometry
    {
        public ElbowIfcGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public ElbowIfcGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static ElbowIfcGeometry Create(IModel model, Elbow elbow)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            FixedVector<Dim3> pA = elbow.PortA.Position;
            FixedVector<Dim3> pB = elbow.PortB.Position;

            FixedVector<Dim3> dA = elbow.PortA.Direction.SafeNormalize();
            FixedVector<Dim3> dB = elbow.PortB.Direction.SafeNormalize();

            FixedVector<Dim3> tangentA = dA.Negate();
            FixedVector<Dim3> tangentB = dB;

            FixedVector<Dim3> normal = tangentA.CreateNormalVector(tangentB).SafeNormalize();

            if (normal.L2Norm() == 0)
                throw new ArgumentException("Elbow port directions must not be parallel.");

            double radius = elbow.Radius;

            FixedVector<Dim3> radialA = tangentA.CreateNormalVector(normal).SafeNormalize();
            FixedVector<Dim3> radialB = tangentB.CreateNormalVector(normal).SafeNormalize();

            FixedVector<Dim3> centerA = pA - radius * radialA;
            FixedVector<Dim3> centerB = pB - radius * radialB;

            FixedVector<Dim3> center = 0.5 * (centerA + centerB);

            FixedVector<Dim3> startRadial = (pA - center).SafeNormalize();
            FixedVector<Dim3> endRadial = (pB - center).SafeNormalize();

            FixedVector<Dim3> u = startRadial;
            FixedVector<Dim3> v = normal.CreateNormalVector(u).SafeNormalize();

            double endAngle = Math.Atan2(endRadial.Dot(v), endRadial.Dot(u));

            if (endAngle < 0)
                endAngle += 2.0 * Math.PI;

            double diameter = elbow.Ports.Max(port => port.Metadata.Diameter);

            TorusSegmentMeshBuilder meshBuilder = new TorusSegmentMeshBuilder(elbow.Radius, diameter / 2.0, 0.0, endAngle);
            meshBuilder
                .Transform(FixedMatrix<Dim4>.Builder.CreateTransition(
                    center - elbow.Position,
                    u,
                    v,
                    normal));

            IMesh mesh = meshBuilder.Build();
            Triangle[] changedTriangles = ChangeTriangleIndexation(mesh.Triangles).ToArray();

            IIfcTriangulatedFaceSetBuilder<IIfcTriangulatedFaceSet> builder = new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
            builder.CreateCoordinates(model, mesh.Vertices);
            builder.AssignTriangleIndices(changedTriangles.Cast<IEnumerable<int>>());
            builder.AssignNormals(mesh.FaceNormals);

            return new ElbowIfcGeometry(builder);
        }
    }
}