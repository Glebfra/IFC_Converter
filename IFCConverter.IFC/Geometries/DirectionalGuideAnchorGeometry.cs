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
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.IFC.Geometries
{
    public struct DirectionalGuideAnchorGeometryProperties
    {
        public Vector<double>[] Positions;
        public Vector<double>[] Directions;
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
                Vector<double> direction = properties.Directions[i];
                Vector<double> topConePoint = properties.Positions[i];
                Vector<double> botConePoint = topConePoint - direction * coneLength;
                Vector<double> botStickPoint = botConePoint - direction * stickLength;
                Vector<double> basePoint = botStickPoint - direction * baseLength;

                Matrix<double> profileDefMatrix =
                    MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);
                Matrix<double> baseExtrudedAreaSolidMatrix =
                    MatrixExtensions.CreateTransition(basePoint, direction);
                Matrix<double> stickExtrudedAreaSolidMatrix =
                    MatrixExtensions.CreateTransition(botStickPoint, direction);

                IIfcRectangleProfileDefBuilder<IfcRectangleProfileDef> baseProfileDefBuilder =
                    new IfcRectangleProfileDefBuilder<IfcRectangleProfileDef>(
                        XDim, YDim, IfcProfileTypeEnum.AREA,
                        $"{nameof(RestingSupportAnchorGeometry)} {nameof(IfcRectangleProfileDef)}"
                    );
                baseProfileDefBuilder.CreatePosition(model, profileDefMatrix);
                IfcRectangleProfileDef baseProfileDef = baseProfileDefBuilder.CreateProfileDef(model);

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