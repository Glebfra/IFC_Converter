using System.Collections.Generic;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Attributes;
using IFCConverter.IFC.Builders.Geometry.Tessellated;
using IFCConverter.IFC.Interfaces;
using IFCConverter.IFC.Interfaces.Geometry.Tessellated;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Geometries
{
    public struct ConeGeometryProperties
    {
        public FixedVector<Dim3> Direction;
        public FixedVector<Dim3>[] Positions;
        public double[] Diameters;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public class ConeGeometry : IfcGeometry
    {
        public ConeGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public ConeGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static ConeGeometry CreateGeometry(IModel model, ConeGeometryProperties properties)
        {
            IfcTriangulatedProperties triangulatedProperties = IfcTriangulatedProperties.CreateClippedCone(
                new ClippedConeTriangulatedGeometryProperties
                {
                    BottomConeCenter = properties.Positions[0],
                    TopConeCenter = properties.Positions[1],
                    BottomDiameter = properties.Diameters[0],
                    TopDiameter = properties.Diameters[1],
                    Direction = properties.Direction
                }
            );

            IIfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet> faceSetBuilder =
                new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
            faceSetBuilder.CreateCoordinates(model, triangulatedProperties.Coordinates);
            faceSetBuilder.AssignTriangleIndices(triangulatedProperties.TriangleIndices);
            faceSetBuilder.AssignNormals(triangulatedProperties.Normals);

            return new ConeGeometry(faceSetBuilder);
        }
    }
}