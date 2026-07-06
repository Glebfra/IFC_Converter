using System.Collections.Generic;
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
    public struct SphericalPipesJointGeometryProperties
    {
        public double PipeDiameter;
        public double SphereDiameter;
        public double Length;
        public Vector<double> Position;
        public Vector<double>[] Points;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public class SphericalPipesJointGeometry : IfcGeometry
    {
        public SphericalPipesJointGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public SphericalPipesJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static SphericalPipesJointGeometry CreateGeometry(IModel model,
            SphericalPipesJointGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new();

            // Creating pipe extrusions
            foreach (Vector<double> point in properties.Points)
            {
                Vector<double> direction = point - properties.Position;
                Matrix<double> extrusionMatrix = MatrixExtensions.CreateTransition(properties.Position, direction);
                Matrix<double> circleProfileDefMatrix =
                    MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);

                IIfcCircleProfileDefBuilder<IfcCircleProfileDef> circleProfileDefBuilder =
                    new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                        properties.PipeDiameter / 2, IfcProfileTypeEnum.AREA,
                        $"{nameof(SphericalPipesJointGeometry)} {nameof(IfcCircleProfileDef)}"
                    );
                circleProfileDefBuilder.CreatePosition(model, circleProfileDefMatrix);
                IfcCircleProfileDef profileDef = circleProfileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                        properties.Length / 2, VectorExtensions.Z, profileDef
                    );
                extrudedAreaSolidBuilder.CreatePosition(model, extrusionMatrix);

                builders.Add(extrudedAreaSolidBuilder);
            }

            // Create sphere
            IfcTriangulatedProperties sphereTriangulatedProperties = IfcTriangulatedProperties.CreateSphere(
                new SphereTriangulatedGeometryProperties
                {
                    Center = properties.Position,
                    Diameter = properties.SphereDiameter
                });
            IIfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet> triangulatedFaceSetBuilder =
                new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
            triangulatedFaceSetBuilder.CreateCoordinates(model, sphereTriangulatedProperties.Coordinates);
            triangulatedFaceSetBuilder.AssignNormals(sphereTriangulatedProperties.Normals);
            triangulatedFaceSetBuilder.AssignTriangleIndices(sphereTriangulatedProperties.TriangleIndices);
            builders.Add(triangulatedFaceSetBuilder);

            return new SphericalPipesJointGeometry(builders);
        }
    }
}