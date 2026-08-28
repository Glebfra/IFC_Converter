using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Geometry;
using IFCConverter.Geometry.MeshBuilders;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Attributes;
using IFCConverter.IFC.Builders.Geometry.Tessellated;
using IFCConverter.IFC.Interfaces;
using IFCConverter.IFC.Interfaces.Geometry.Tessellated;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Geometries
{
    public struct BendTriangulatedGeometryProperties
    {
        public double PipeDiameter;

        public Vector<double> Position;
        public Vector<double> StartArcPosition;
        public Vector<double> EndArcPosition;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public sealed class BendTriangulatedGeometry : IfcGeometry
    {
        public BendTriangulatedGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null) : base(geometryBuilder,
            representationContext)
        {
        }

        public BendTriangulatedGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null) : base(
            geometryBuilders, representationContext)
        {
        }

        [Pure]
        public static BendTriangulatedGeometry CreateGeometry(IModel model, BendTriangulatedGeometryProperties properties)
        {
            IMeshBuilder meshBuilder = new TorusSegmentMeshBuilder(
                properties.Position,
                properties.StartArcPosition,
                properties.EndArcPosition,
                properties.PipeDiameter / 2);

            IMesh mesh = meshBuilder.Build();
            int[][] triangles = ChangeTriangleIndexation(mesh.Triangles);

            IIfcTriangulatedFaceSetBuilder<IIfcTriangulatedFaceSet> builder = new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
            builder.CreateCoordinates(model, mesh.Vertices);
            builder.AssignTriangleIndices(triangles);
            builder.AssignNormals(mesh.Normals);

            return new BendTriangulatedGeometry(builder);
        }
    }
}