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
    public struct TorsionExpansionJointGeometryProperties
    {
        public Vector<double> Position;
        public Vector<double>[] Points;
        public double Diameter;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    public class TorsionExpansionJointGeometry : IfcGeometry
    {
        private const double DiameterToBottomConeDiameterFactor = 1.25;

        public TorsionExpansionJointGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public TorsionExpansionJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static TorsionExpansionJointGeometry CreateGeometry(IModel model,
            TorsionExpansionJointGeometryProperties properties)
        {
            Vector<double>[] directions = properties.Points
                .Select(point => point - properties.Position)
                .ToArray();
            double[] lengths = directions.Select(direction => direction.L2Norm()).ToArray();
            double[] segmentLengths = lengths.Select(length => length / 2.5).ToArray();

            List<IIfcBuilder> builders = new();
            for (int i = 0; i < directions.Length; i++)
            {
                Vector<double> direction = directions[i].Normalize(2);
                Vector<double> extrusionPoint = properties.Position + direction * segmentLengths[i] / 2;
                Matrix<double> extrusionMatrix = MatrixExtensions.CreateTransition(extrusionPoint, direction);
                Matrix<double> profileDefMatrix =
                    MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);

                IIfcCircleProfileDefBuilder<IfcCircleProfileDef> profileDefBuilder =
                    new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                        properties.Diameter, IfcProfileTypeEnum.AREA,
                        $"{nameof(TorsionExpansionJointGeometry)} {nameof(IfcCircleProfileDef)}");
                profileDefBuilder.CreatePosition(model, profileDefMatrix);
                IfcCircleProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                        segmentLengths[i], VectorExtensions.Z, profileDef
                    );
                extrudedAreaSolidBuilder.CreatePosition(model, extrusionMatrix);

                Vector<double> bottomConePoint = extrusionPoint + direction * segmentLengths[i];
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