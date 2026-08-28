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
    public struct LateralExpansionJointGeometryProperties
    {
        public Vector<double>[] Points;
        public Vector<double> Position;
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

        public static LateralExpansionJointGeometry CreateGeometry(IModel model,
            LateralExpansionJointGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            Vector<double> direction = (properties.Points[0] - properties.Position).Normalize(2);
            double length = (properties.Points[1] - properties.Points[0]).L2Norm();

            Vector<double> extrudedPoint = properties.Position - direction * (length / 2);

            Matrix<double> extrudedMatrix = MatrixExtensions.CreateTransition(extrudedPoint, direction);
            Matrix<double> profileDefMatrix =
                MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);

            IIfcCircleProfileDefBuilder<IfcCircleProfileDef> profileDefBuilder =
                new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                    properties.Diameter / 2, IfcProfileTypeEnum.AREA,
                    $"{nameof(LateralExpansionJointGeometry)} {nameof(IfcCircleProfileDef)}"
                );
            profileDefBuilder.CreatePosition(model, profileDefMatrix);
            IfcCircleProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                    length, VectorExtensions.Z, profileDef
                );
            extrudedAreaSolidBuilder.CreatePosition(model, extrudedMatrix);
            builders.Add(extrudedAreaSolidBuilder);

            Vector<double>[] sphereCenters =
            {
                properties.Position + direction * length / 4, properties.Position - direction * length / 4
            };
            foreach (Vector<double> sphereCenter in sphereCenters)
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