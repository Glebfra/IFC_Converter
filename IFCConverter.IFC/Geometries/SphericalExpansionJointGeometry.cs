using System.Collections.Generic;
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
    public struct BallExpansionJointGeometryProperties
    {
        public Vector<double> Position;
        public double Diameter;
    }
    
    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public sealed class BallExpansionJointGeometry : IfcGeometry
    {
        public BallExpansionJointGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null) 
            : base(geometryBuilder, representationContext)
        {
        }

        public BallExpansionJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null) 
            : base(geometryBuilders, representationContext)
        {
        }

        public static BallExpansionJointGeometry CreateGeometry(IModel model, BallExpansionJointGeometryProperties properties)
        {
            IfcTriangulatedProperties triangulatedProperties = IfcTriangulatedProperties.CreateSphere(new SphereTriangulatedGeometryProperties()
            {
                Center = properties.Position,
                Diameter = properties.Diameter
            });

            IIfcTriangulatedFaceSetBuilder<IIfcTriangulatedFaceSet> builder = new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
            builder.CreateCoordinates(model, triangulatedProperties.Coordinates);
            builder.AssignTriangleIndices(triangulatedProperties.TriangleIndices);
            builder.AssignNormals(triangulatedProperties.Normals);

            return new BallExpansionJointGeometry(builder);
        }
    }
}