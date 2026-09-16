using System.Collections.Generic;
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
    public struct DirectionalGuideAnchorGeometryProperties
    {
        public FixedVector<Dim3>[] Positions;
        public FixedVector<Dim3>[] Directions;
        public double Diameter;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public class DirectionalGuideAnchorGeometry : IfcGeometry
    {
        private const double DiameterToLengthFactor = 1.5;
        private const double DiameterToBaseXDimFactor = 0.5;
        private const double XDimToYDimFactor = 1.0;
        private const double DiameterToConeDiameterFactor = 0.5;
        private const double DiameterToStickDiameterFactor = 0.2;

        public DirectionalGuideAnchorGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public DirectionalGuideAnchorGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static DirectionalGuideAnchorGeometry CreateGeometry(IModel model,
            DirectionalGuideAnchorGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            double length = properties.Diameter * DiameterToLengthFactor;
            double baseLength = length / 10;
            double coneLength = (length - baseLength) / 2;
            double stickLength = coneLength;

            double XDim = properties.Diameter * DiameterToBaseXDimFactor;
            double YDim = XDim * XDimToYDimFactor;

            double coneDiameter = properties.Diameter * DiameterToConeDiameterFactor;
            double stickDiameter = properties.Diameter * DiameterToStickDiameterFactor;

            for (int i = 0; i < properties.Positions.Length; i++)
            {
                FixedVector<Dim3> direction = properties.Directions[i];
                FixedVector<Dim3> topConePoint = properties.Positions[i];
                FixedVector<Dim3> botConePoint = topConePoint - direction * coneLength;
                FixedVector<Dim3> botStickPoint = botConePoint - direction * stickLength;
                FixedVector<Dim3> basePoint = botStickPoint - direction * baseLength;

                FixedVector<Dim3> zAxis = direction;
                FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
                FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

                FixedMatrix<Dim4> profileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
                FixedMatrix<Dim4> baseExtrudedAreaSolidMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(basePoint, xAxis, yAxis, zAxis);
                FixedMatrix<Dim4> stickExtrudedAreaSolidMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(botStickPoint, xAxis, yAxis, zAxis);

                IIfcRectangleProfileDefBuilder<IfcRectangleProfileDef> baseProfileDefBuilder =
                    new IfcRectangleProfileDefBuilder<IfcRectangleProfileDef>(
                        XDim, YDim, IfcProfileTypeEnum.AREA,
                        $"{nameof(RestingSupportAnchorGeometry)} {nameof(IfcRectangleProfileDef)}"
                    );
                baseProfileDefBuilder.CreatePosition(model, profileDefMatrix);
                IfcRectangleProfileDef baseProfileDef = baseProfileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> baseExtrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(baseLength, FixedVector<Dim3>.Builder.Z(),
                        baseProfileDef);
                baseExtrudedAreaSolidBuilder.CreatePosition(model, baseExtrudedAreaSolidMatrix);
                builders.Add(baseExtrudedAreaSolidBuilder);

                IIfcCircleProfileDefBuilder<IfcCircleProfileDef> stickProfileDefBuilder =
                    new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                        stickDiameter / 2, IfcProfileTypeEnum.AREA,
                        $"{nameof(RestingSupportAnchorGeometry)} {nameof(IfcCircleProfileDef)}"
                    );
                stickProfileDefBuilder.CreatePosition(model, profileDefMatrix);
                IfcCircleProfileDef stickProfileDef = stickProfileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> stickExtrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(stickLength, FixedVector<Dim3>.Builder.Z(),
                        stickProfileDef);
                stickExtrudedAreaSolidBuilder.CreatePosition(model, stickExtrudedAreaSolidMatrix);
                builders.Add(stickExtrudedAreaSolidBuilder);

                IfcTriangulatedProperties coneProperties = IfcTriangulatedProperties.CreateCone(
                    new ConeTriangulatedGeometryProperties
                    {
                        Diameter = coneDiameter,
                        BottomConeCenter = botConePoint,
                        TopConePoint = topConePoint
                    });
                IIfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet> coneTriangulatedFaceSetBuilder =
                    new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
                coneTriangulatedFaceSetBuilder.CreateCoordinates(model, coneProperties.Coordinates);
                coneTriangulatedFaceSetBuilder.AssignNormals(coneProperties.Normals);
                coneTriangulatedFaceSetBuilder.AssignTriangleIndices(coneProperties.TriangleIndices);
                builders.Add(coneTriangulatedFaceSetBuilder);
            }

            return new DirectionalGuideAnchorGeometry(builders);
        }
    }
}