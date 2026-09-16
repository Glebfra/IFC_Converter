using System.Collections.Generic;
using System.Linq;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Attributes;
using IFCConverter.IFC.Builders.Geometry.ProfileDef;
using IFCConverter.IFC.Builders.Geometry.SolidModel;
using IFCConverter.IFC.Builders.Geometry.Tessellated;
using IFCConverter.IFC.Interfaces;
using IFCConverter.IFC.Interfaces.Geometry.ProfileDef;
using IFCConverter.IFC.Interfaces.Geometry.SolidModel;
using IFCConverter.IFC.Interfaces.Geometry.Tessellated;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;

namespace IFCConverter.IFC.Geometries
{
    public struct TorsionExpansionJointGeometryProperties
    {
        public FixedVector<Dim3> Position;
        public FixedVector<Dim3>[] Points;
        public double Diameter;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    public class TorsionExpansionJointGeometry : IfcGeometry
    {
        private const double DiameterToBottomConeDiameterFactor = 1.25;

        public TorsionExpansionJointGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public TorsionExpansionJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static TorsionExpansionJointGeometry CreateGeometry(IModel model,
            TorsionExpansionJointGeometryProperties properties)
        {
            FixedVector<Dim3>[] directions = properties.Points
                .Select(point => point - properties.Position)
                .ToArray();
            double[] lengths = directions.Select(direction => direction.L2Norm()).ToArray();
            double[] segmentLengths = lengths.Select(length => length / 2.5).ToArray();

            List<IIfcBuilder> builders = new List<IIfcBuilder>();
            for (int i = 0; i < directions.Length; i++)
            {
                FixedVector<Dim3> direction = directions[i].Normalize();
                FixedVector<Dim3> extrusionPoint = properties.Position + direction * (segmentLengths[i] / 2);

                FixedVector<Dim3> zAxis = direction;
                FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
                FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

                FixedMatrix<Dim4> profileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
                FixedMatrix<Dim4> extrusionMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(extrusionPoint, xAxis, yAxis, zAxis);

                IIfcCircleProfileDefBuilder<IfcCircleProfileDef> profileDefBuilder =
                    new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                        properties.Diameter, IfcProfileTypeEnum.AREA,
                        $"{nameof(TorsionExpansionJointGeometry)} {nameof(IfcCircleProfileDef)}");
                profileDefBuilder.CreatePosition(model, profileDefMatrix);
                IfcCircleProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                        segmentLengths[i], FixedVector<Dim3>.Builder.Z(), profileDef
                    );
                extrudedAreaSolidBuilder.CreatePosition(model, extrusionMatrix);

                FixedVector<Dim3> bottomConePoint = extrusionPoint + direction * segmentLengths[i];
                IfcTriangulatedProperties coneProperties = IfcTriangulatedProperties.CreateClippedCone(
                    new ClippedConeTriangulatedGeometryProperties
                    {
                        BottomDiameter = properties.Diameter * DiameterToBottomConeDiameterFactor,
                        TopDiameter = properties.Diameter,
                        Direction = direction,
                        BottomConeCenter = bottomConePoint,
                        TopConeCenter = properties.Points[i]
                    });
                IIfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet> triangulatedFaceSetBuilder =
                    new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
                triangulatedFaceSetBuilder.CreateCoordinates(model, coneProperties.Coordinates);
                triangulatedFaceSetBuilder.AssignNormals(coneProperties.Normals);
                triangulatedFaceSetBuilder.AssignTriangleIndices(coneProperties.TriangleIndices);

                builders.Add(extrudedAreaSolidBuilder);
                builders.Add(triangulatedFaceSetBuilder);
            }

            return new TorsionExpansionJointGeometry(builders);
        }
    }
}