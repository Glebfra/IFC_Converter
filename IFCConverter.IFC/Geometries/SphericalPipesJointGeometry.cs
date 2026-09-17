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
    public struct SphericalPipesJointGeometryProperties
    {
        public double PipeDiameter;
        public double SphereDiameter;
        public double Length;
        public FixedVector<Dim3> Position;
        public FixedVector<Dim3>[] Points;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public class SphericalPipesJointGeometry : IfcGeometry
    {
        public SphericalPipesJointGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public SphericalPipesJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static SphericalPipesJointGeometry CreateGeometry(IModel model,
            SphericalPipesJointGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            foreach (FixedVector<Dim3> point in properties.Points)
            {
                FixedVector<Dim3> direction = point - properties.Position;

                FixedVector<Dim3> zAxis = direction;
                FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
                FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

                FixedMatrix<Dim4> circleProfileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
                FixedMatrix<Dim4> extrusionMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(properties.Position, xAxis, yAxis, zAxis);

                IIfcCircleProfileDefBuilder<IfcCircleProfileDef> circleProfileDefBuilder =
                    new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                        properties.PipeDiameter / 2, IfcProfileTypeEnum.AREA,
                        $"{nameof(SphericalPipesJointGeometry)} {nameof(IfcCircleProfileDef)}"
                    );
                circleProfileDefBuilder.CreatePosition(model, circleProfileDefMatrix);
                IfcCircleProfileDef profileDef = circleProfileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                        properties.Length / 2, FixedVector<Dim3>.Builder.Z(), profileDef
                    );
                extrudedAreaSolidBuilder.CreatePosition(model, extrusionMatrix);

                builders.Add(extrudedAreaSolidBuilder);
            }

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