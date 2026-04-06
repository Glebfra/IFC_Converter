using System.Collections.Generic;
using System.Linq;
using Ifc.API;
using Ifc.Attributes;
using Ifc.Builders.Geometry.ProfileDef;
using Ifc.Builders.Geometry.SolidModel;
using Ifc.Builders.Geometry.Tessellated;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace Ifc.Geometries
{
    public struct ConstantSpringSupportAnchorGeometryProperties
    {
        public Vector<double> Position;
        public Vector<double> Direction;
        public double Diameter;
        public bool IsDoubleSided;
        public Vector<double> DoubleSidedDisplacement;
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
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public ConstantSpringAnchorGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static ConstantSpringAnchorGeometry CreateGeometry(IModel model,
            ConstantSpringSupportAnchorGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new();

            double length = properties.Diameter * DiameterToLengthFactor;
            double baseLength = length * LengthToBaseLengthFactor;
            double coneLength = (length - baseLength) / 3;
            double stickLength = coneLength;

            double XDim = properties.Diameter * DiameterToBaseXDimFactor;
            double YDim = XDim * XDimToYDimFactor;

            double coneDiameter = properties.Diameter * DiameterToConeDiameterFactor;
            double stickDiameter = properties.Diameter * DiameterToStickDiameterFactor;

            Vector<double>[] topConeTopPoints = properties.IsDoubleSided
                ? new[]
                {
                    properties.Position + properties.DoubleSidedDisplacement,
                    properties.Position - properties.DoubleSidedDisplacement
                }
                : new[] { properties.Position };

            Vector<double>[] topConeBotPoints = topConeTopPoints
                .Select(topConePoint => topConePoint - properties.Direction * coneLength)
                .ToArray();
            Vector<double>[] botStickPoints = topConeBotPoints
                .Select(botConePoint => botConePoint - properties.Direction * stickLength)
                .ToArray();
            Vector<double>[] botConeBotPoints = botStickPoints;
            Vector<double>[] botConeTopPoints = botConeBotPoints
                .Select(botConeBotPoint => botConeBotPoint - properties.Direction * coneLength)
                .ToArray();
            Vector<double>[] basePoints = botConeTopPoints
                .Select(botConeTopPoint => botConeTopPoint - properties.Direction * baseLength)
                .ToArray();

            for (int i = 0; i < topConeTopPoints.Length; i++)
            {
                Vector<double> topConeTopPoint = topConeTopPoints[i];
                Vector<double> topConeBotPoint = topConeBotPoints[i];
                Vector<double> botStickPoint = botStickPoints[i];
                Vector<double> botConeBotPoint = botConeBotPoints[i];
                Vector<double> botConeTopPoint = botConeTopPoints[i];
                Vector<double> basePoint = basePoints[i];

                Matrix<double> profileDefMatrix =
                    MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);
                Matrix<double> baseExtrudedAreaSolidMatrix =
                    MatrixExtensions.CreateTransition(basePoint, properties.Direction);
                Matrix<double> stickExtrudedAreaSolidMatrix =
                    MatrixExtensions.CreateTransition(botStickPoint, properties.Direction);

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
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(baseLength, VectorExtensions.Z,
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
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(stickLength, VectorExtensions.Z,
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