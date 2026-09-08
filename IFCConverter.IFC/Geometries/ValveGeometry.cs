using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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
    public struct ValveGeometryProperties
    {
        public double Diameter;
        public double Length;
        public Vector<double> TopConePoint;
        public Vector<double>[] BotConePoints;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public class ValveGeometry : IfcGeometry
    {
        private const double DiameterToConeDiameterFactor = 1.5;

        public ValveGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public ValveGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        [Pure]
        public static ValveGeometry CreateGeometry(IModel model, ValveGeometryProperties properties)
        {
            IEnumerable<IfcTriangulatedProperties> triangulatedProperties = properties.BotConePoints
                .Select(botConePoint => IfcTriangulatedProperties.CreateCone(new ConeTriangulatedGeometryProperties
                {
                    TopConePoint = properties.TopConePoint,
                    BottomConeCenter = botConePoint,
                    Diameter = properties.Diameter * DiameterToConeDiameterFactor
                }));

            IEnumerable<IIfcBuilder> builders = triangulatedProperties
                .Select(triangulatedProp => IfcTriangulatedFaceSetBuilder(model, triangulatedProp));

            return new ValveGeometry(builders);
        }

        private static IIfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet> IfcTriangulatedFaceSetBuilder(
            IModel model,
            IfcTriangulatedProperties triangulatedProp)
        {
            IIfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet> triangulatedFaceSetBuilder =
                new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
            triangulatedFaceSetBuilder.CreateCoordinates(model, triangulatedProp.Coordinates);
            triangulatedFaceSetBuilder.AssignTriangleIndices(triangulatedProp.TriangleIndices);
            triangulatedFaceSetBuilder.AssignNormals(triangulatedProp.Normals);
            return triangulatedFaceSetBuilder;
        }
    }
}