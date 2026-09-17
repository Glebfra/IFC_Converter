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
    public struct LateralExpansionJointGeometryProperties
    {
        public FixedVector<Dim3>[] Points;
        public FixedVector<Dim3> Position;
        public double Diameter;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    public class LateralExpansionJointGeometry : IfcGeometry
    {
        private const double DiameterToSphereDiameterFactor = 1.25;

        public LateralExpansionJointGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public LateralExpansionJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static LateralExpansionJointGeometry CreateGeometry(IModel model, LateralExpansionJointGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            FixedVector<Dim3> direction = (properties.Points[0] - properties.Position).Normalize();
            double length = (properties.Points[1] - properties.Points[0]).L2Norm();

            FixedVector<Dim3> extrudedPoint = properties.Position - direction * (length / 2);

            FixedVector<Dim3> zAxis = direction;
            FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
            FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

            FixedMatrix<Dim4> profileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
            FixedMatrix<Dim4> extrudedMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(extrudedPoint, xAxis, yAxis, zAxis);

            IIfcCircleProfileDefBuilder<IfcCircleProfileDef> profileDefBuilder =
                new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                    properties.Diameter / 2, IfcProfileTypeEnum.AREA,
                    $"{nameof(LateralExpansionJointGeometry)} {nameof(IfcCircleProfileDef)}"
                );
            profileDefBuilder.CreatePosition(model, profileDefMatrix);
            IfcCircleProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                    length, FixedVector<Dim3>.Builder.Z(), profileDef
                );
            extrudedAreaSolidBuilder.CreatePosition(model, extrudedMatrix);
            builders.Add(extrudedAreaSolidBuilder);

            FixedVector<Dim3>[] sphereCenters =
            {
                properties.Position + direction * (length / 4), properties.Position - direction * (length / 4)
            };
            foreach (FixedVector<Dim3> sphereCenter in sphereCenters)
            {
                IfcTriangulatedProperties triangulatedProperties = IfcTriangulatedProperties.CreateSphere(
                    new SphereTriangulatedGeometryProperties
                    {
                        Center = sphereCenter,
                        Diameter = properties.Diameter * DiameterToSphereDiameterFactor
                    });
                IIfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet> faceSetBuilder =
                    new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
                faceSetBuilder.CreateCoordinates(model, triangulatedProperties.Coordinates);
                faceSetBuilder.AssignNormals(triangulatedProperties.Normals);
                faceSetBuilder.AssignTriangleIndices(triangulatedProperties.TriangleIndices);

                builders.Add(faceSetBuilder);
            }

            return new LateralExpansionJointGeometry(builders);
        }
    }
}