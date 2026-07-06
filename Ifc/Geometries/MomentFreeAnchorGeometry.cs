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
    public struct HingedAnchorGeometryProperties
    {
        public Vector<double> Position;
        public Vector<double> Direction;
        public double Diameter;
        public bool IsDoubleSided;
        public Vector<double> DoubleSidedDisplacement;
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
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public MomentFreeAnchorGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static MomentFreeAnchorGeometry CreateGeometry(IModel model, HingedAnchorGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new();

            double length = properties.Diameter * DiameterToLengthFactor;
            double baseLength = length / 10;

            double XDim = properties.Diameter * DiameterToBaseXDimFactor;
            double YDim = XDim * XDimToYDimFactor;

            double coneDiameter = properties.Diameter * DiameterToConeDiameterFactor;

            Vector<double>[] topConePoints = properties.IsDoubleSided
                ? new[]
                {
                    properties.Position + properties.DoubleSidedDisplacement, properties.Position - properties.DoubleSidedDisplacement
                }
                : new[]
                {
                    properties.Position
                };
            Vector<double>[] botConePoints = topConePoints
                .Select(topConePoint => topConePoint - properties.Direction * (length - baseLength))
                .ToArray();
            Vector<double>[] basePoints = botConePoints
                .Select(botConePoint => botConePoint - properties.Direction * baseLength)
                .ToArray();

            for (int i = 0; i < topConePoints.Length; i++)
            {
                Vector<double> topConePoint = topConePoints[i];
                Vector<double> botConePoint = botConePoints[i];
                Vector<double> basePoint = basePoints[i];

                Matrix<double> baseExtrudedAreaSolidMatrix =
                    MatrixExtensions.CreateTransition(basePoint, properties.Direction);
                Matrix<double> baseProfileDefMatrix =
                    MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);

                IIfcRectangleProfileDefBuilder<IfcRectangleProfileDef> baseProfileDefBuilder =
                    new IfcRectangleProfileDefBuilder<IfcRectangleProfileDef>(
                        XDim, YDim, IfcProfileTypeEnum.AREA,
                        $"{nameof(MomentFreeAnchorGeometry)} {nameof(IfcRectangleProfileDef)}"
                    );
                baseProfileDefBuilder.CreatePosition(model, baseProfileDefMatrix);
                IfcRectangleProfileDef baseProfileDef = baseProfileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> baseExtrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(baseLength, VectorExtensions.Z,
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