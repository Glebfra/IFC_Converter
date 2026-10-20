using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;
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
    internal sealed class ValveIfcGeometry : IfcGeometry
    {
        private const double RadiusFactor = 1.5 * 0.5;

        public ValveIfcGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null) 
            : base(geometryBuilder, representationContext)
        {
        }

        public ValveIfcGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null) 
            : base(geometryBuilders, representationContext)
        {
        }

        public static ValveIfcGeometry Create(IModel model, Valve valve)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();
            
            double radius = valve.Ports.Max(port => port.Metadata.Diameter * RadiusFactor);
            
            foreach (Port port in valve.Ports)
            {
                FixedVector<Dim3> projection = valve.Position - port.Position;
                FixedVector<Dim3> direction = projection.SafeNormalize();
                FixedVector<Dim3> rightDirection = direction.CreateNormalVector().SafeNormalize();
                FixedVector<Dim3> upDirection = direction.CreateNormalVector(rightDirection).SafeNormalize();

                double height = projection.L2Norm();

                ConeMeshBuilder meshBuilder = new ConeMeshBuilder(radius, height);
                meshBuilder
                    .Transform(FixedMatrix<Dim4>.Builder.CreateTransition(
                        projection.Negate(), 
                        rightDirection,
                        upDirection,
                        direction))
                    .AddPolygon(meshBuilder.RingIndices);
                
                IMesh mesh = meshBuilder.Build();
                Triangle[] changedTriangles = ChangeTriangleIndexation(mesh.Triangles).ToArray();

                IIfcTriangulatedFaceSetBuilder<IIfcTriangulatedFaceSet> builder = new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
                builder.CreateCoordinates(model, mesh.Vertices);
                builder.AssignTriangleIndices(changedTriangles.Cast<IEnumerable<int>>());
                builder.AssignNormals(mesh.FaceNormals);
                
                builders.Add(builder);
            }
            
            return new ValveIfcGeometry(builders);
        }
    }
}