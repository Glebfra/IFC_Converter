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
    public struct ConstantSpringSupportAnchorGeometryProperties
    {
        public FixedVector<Dim3> Position;
        public FixedVector<Dim3> Direction;
        public double Diameter;
        public bool IsDoubleSided;
        public FixedVector<Dim3> DoubleSidedDisplacement;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public class ConstantSpringAnchorGeometry : IfcGeometry
    {
        private const double LengthToBaseLengthFactor = 0.1;
        private const double DiameterToLengthFactor = 1.75;
        private const double DiameterToBaseXDimFactor = 1.5;
        private const double XDimToYDimFactor = 0.5;
        private const double DiameterToConeDiameterFactor = 0.5;
        private const double DiameterToStickDiameterFactor = 0.2;

        public ConstantSpringAnchorGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public ConstantSpringAnchorGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static ConstantSpringAnchorGeometry CreateGeometry(IModel model,
            ConstantSpringSupportAnchorGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            double length = properties.Diameter * DiameterToLengthFactor;
            double baseLength = length * LengthToBaseLengthFactor;
            double coneLength = (length - baseLength) / 3;
            double stickLength = coneLength;

            double XDim = properties.Diameter * DiameterToBaseXDimFactor;
            double YDim = XDim * XDimToYDimFactor;

            double coneDiameter = properties.Diameter * DiameterToConeDiameterFactor;
            double stickDiameter = properties.Diameter * DiameterToStickDiameterFactor;

            FixedVector<Dim3>[] topConeTopPoints = properties.IsDoubleSided
                ? new[]
                {
                    properties.Position + properties.DoubleSidedDisplacement, properties.Position - properties.DoubleSidedDisplacement
                }
                : new[]
                {
                    properties.Position
                };

            FixedVector<Dim3>[] topConeBotPoints = topConeTopPoints
                .Select(topConePoint => topConePoint - properties.Direction * coneLength)
                .ToArray();
            FixedVector<Dim3>[] botStickPoints = topConeBotPoints
                .Select(botConePoint => botConePoint - properties.Direction * stickLength)
                .ToArray();
            FixedVector<Dim3>[] botConeBotPoints = botStickPoints;
            FixedVector<Dim3>[] botConeTopPoints = botConeBotPoints
                .Select(botConeBotPoint => botConeBotPoint - properties.Direction * coneLength)
                .ToArray();
            FixedVector<Dim3>[] basePoints = botConeTopPoints
                .Select(botConeTopPoint => botConeTopPoint - properties.Direction * baseLength)
                .ToArray();

            for (int i = 0; i < topConeTopPoints.Length; i++)
            {
                FixedVector<Dim3> topConeTopPoint = topConeTopPoints[i];
                FixedVector<Dim3> topConeBotPoint = topConeBotPoints[i];
                FixedVector<Dim3> botStickPoint = botStickPoints[i];
                FixedVector<Dim3> botConeBotPoint = botConeBotPoints[i];
                FixedVector<Dim3> botConeTopPoint = botConeTopPoints[i];
                FixedVector<Dim3> basePoint = basePoints[i];

                FixedVector<Dim3> profileDefXAxis = FixedVector<Dim3>.Builder.X();
                FixedVector<Dim3> profileDefYAxis = FixedVector<Dim3>.Builder.Y();
                FixedVector<Dim3> profileDefZAxis = FixedVector<Dim3>.Builder.Z();
                FixedMatrix<Dim4> profileDefMatrix =
                    FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros(), profileDefXAxis, profileDefYAxis, profileDefZAxis);

                FixedVector<Dim3> zAxis = properties.Direction;
                FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
                FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

                FixedMatrix<Dim4> baseExtrudedAreaSolidMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(basePoint, xAxis, yAxis, zAxis);
                FixedMatrix<Dim4> stickExtrudedAreaSolidMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(botStickPoint, xAxis, yAxis, zAxis);

                IIfcRectangleProfileDefBuilder<IfcRectangleProfileDef> baseProfileDefBuilder =
                    new IfcRectangleProfileDefBuilder<IfcRectangleProfileDef>(
                        XDim, YDim, IfcProfileTypeEnum.AREA,
                        $"{nameof(RestingSupportAnchorGeometry)} {nameof(IfcRectangleProfileDef)}"
                    );
                baseProfileDefBuilder.CreatePosition(model, profileDefMatrix);
                IfcRectangleProfileDef baseProfileDef = baseProfileDefBuilder.CreateProfileDef(model);

                IfcTriangulatedProperties botConeProperties = IfcTriangulatedProperties.CreateCone(
                    new ConeTriangulatedGeometryProperties
                    {
                        Diameter = coneDiameter,
                        BottomConeCenter = botConeBotPoint,
                        TopConePoint = botConeTopPoint
                    });
                IIfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet> botConeTriangulatedFaceSetBuilder =
                    new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
                botConeTriangulatedFaceSetBuilder.CreateCoordinates(model, botConeProperties.Coordinates);
                botConeTriangulatedFaceSetBuilder.AssignNormals(botConeProperties.Normals);
                botConeTriangulatedFaceSetBuilder.AssignTriangleIndices(botConeProperties.TriangleIndices);
                builders.Add(botConeTriangulatedFaceSetBuilder);

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

                IfcTriangulatedProperties topConeProperties = IfcTriangulatedProperties.CreateCone(
                    new ConeTriangulatedGeometryProperties
                    {
                        Diameter = coneDiameter,
                        BottomConeCenter = topConeBotPoint,
                        TopConePoint = topConeTopPoint
                    });
                IIfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet> topConeTriangulatedFaceSetBuilder =
                    new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
                topConeTriangulatedFaceSetBuilder.CreateCoordinates(model, topConeProperties.Coordinates);
                topConeTriangulatedFaceSetBuilder.AssignNormals(topConeProperties.Normals);
                topConeTriangulatedFaceSetBuilder.AssignTriangleIndices(topConeProperties.TriangleIndices);
                builders.Add(topConeTriangulatedFaceSetBuilder);
            }

            return new ConstantSpringAnchorGeometry(builders);
        }
    }
}