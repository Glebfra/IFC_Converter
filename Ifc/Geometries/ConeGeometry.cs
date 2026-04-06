using System.Collections.Generic;
using Ifc.API;
using Ifc.Attributes;
using Ifc.Builders.Geometry.Tessellated;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using VectorExtensions = Utils.VectorExtensions;

namespace Ifc.Geometries
{
    public struct ConeGeometryProperties
    {
        public Vector<double> Direction;
        public Vector<double>[] Positions;
        public double[] Diameters;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public class ConeGeometry : IfcGeometry
    {
        public ConeGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public ConeGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static ConeGeometry CreateGeometry(IModel model, ConeGeometryProperties properties)
        {
            Vector<double> direction = properties.Positions[1] - properties.Positions[0];

            IfcTriangulatedProperties triangulatedProperties = IfcTriangulatedProperties.CreateClippedCone(
                new ClippedConeTriangulatedGeometryProperties
                {
                    BottomConeCenter = VectorExtensions.Zero,
                    TopConeCenter = direction,
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