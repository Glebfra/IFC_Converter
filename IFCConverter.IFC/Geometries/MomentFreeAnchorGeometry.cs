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
    public struct HingedAnchorGeometryProperties
    {
        public FixedVector<Dim3> Position;
        public FixedVector<Dim3> Direction;
        public double Diameter;
        public bool IsDoubleSided;
        public FixedVector<Dim3> DoubleSidedDisplacement;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public class MomentFreeAnchorGeometry : IfcGeometry
    {
        private const double DiameterToLengthFactor = 1.5;
        private const double DiameterToBaseXDimFactor = 1.5;
        private const double XDimToYDimFactor = 1.0;
        private const double DiameterToConeDiameterFactor = 1.0;

        public MomentFreeAnchorGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public MomentFreeAnchorGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static MomentFreeAnchorGeometry CreateGeometry(IModel model, HingedAnchorGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            double length = properties.Diameter * DiameterToLengthFactor;
            double baseLength = length / 10;

            double XDim = properties.Diameter * DiameterToBaseXDimFactor;
            double YDim = XDim * XDimToYDimFactor;

            double coneDiameter = properties.Diameter * DiameterToConeDiameterFactor;

            FixedVector<Dim3>[] topConePoints = properties.IsDoubleSided
                ? new[]
                {
                    properties.Position + properties.DoubleSidedDisplacement, properties.Position - properties.DoubleSidedDisplacement
                }
                : new[]
                {
                    properties.Position
                };
            FixedVector<Dim3>[] botConePoints = topConePoints
                .Select(topConePoint => topConePoint - properties.Direction * (length - baseLength))
                .ToArray();
            FixedVector<Dim3>[] basePoints = botConePoints
                .Select(botConePoint => botConePoint - properties.Direction * baseLength)
                .ToArray();

            for (int i = 0; i < topConePoints.Length; i++)
            {
                FixedVector<Dim3> topConePoint = topConePoints[i];
                FixedVector<Dim3> botConePoint = botConePoints[i];
                FixedVector<Dim3> basePoint = basePoints[i];

                FixedVector<Dim3> zAxis = properties.Direction;
                FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
                FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

                FixedMatrix<Dim4> baseProfileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
                FixedMatrix<Dim4> baseExtrudedAreaSolidMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(basePoint, xAxis, yAxis, zAxis);

                IIfcRectangleProfileDefBuilder<IfcRectangleProfileDef> baseProfileDefBuilder =
                    new IfcRectangleProfileDefBuilder<IfcRectangleProfileDef>(
                        XDim, YDim, IfcProfileTypeEnum.AREA,
                        $"{nameof(MomentFreeAnchorGeometry)} {nameof(IfcRectangleProfileDef)}"
                    );
                baseProfileDefBuilder.CreatePosition(model, baseProfileDefMatrix);
                IfcRectangleProfileDef baseProfileDef = baseProfileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> baseExtrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(baseLength, FixedVector<Dim3>.Builder.Z(),
                        baseProfileDef);
                baseExtrudedAreaSolidBuilder.CreatePosition(model, baseExtrudedAreaSolidMatrix);
                builders.Add(baseExtrudedAreaSolidBuilder);

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

            return new MomentFreeAnchorGeometry(builders);
        }
    }
}