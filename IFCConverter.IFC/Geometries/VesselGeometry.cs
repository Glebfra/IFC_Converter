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
    public struct VesselGeometryProperties
    {
        public FixedVector<Dim3>[] Points { get; set; }
        public double Diameter { get; set; }
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public sealed class VesselGeometry : IfcGeometry
    {
        private const double GeometrySectorLengthFactor = 0.5;
        private const double ConeMaxDiameterFactor = 1.1;
        private const double DiameterExtrudedAreaSolidFactor = 1.3;

        public VesselGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null) : base(geometryBuilder,
            representationContext)
        {
        }

        public VesselGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null) : base(geometryBuilders,
            representationContext)
        {
        }

        public static VesselGeometry CreateGeometry(IModel model, VesselGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            FixedVector<Dim3> projection = properties.Points[1] - properties.Points[0];
            double length = projection.L2Norm();
            FixedVector<Dim3> direction = projection * (1 / length);
            FixedVector<Dim3> sectorDisplacement = direction * length * GeometrySectorLengthFactor;

            double extrudedAreaSolidDiameter = properties.Diameter * DiameterExtrudedAreaSolidFactor;
            double minConeDiameter = properties.Diameter;
            double maxConeDiameter = minConeDiameter * ConeMaxDiameterFactor;

            FixedVector<Dim3> startConePoint = properties.Points[0];
            FixedVector<Dim3> endConePoint = startConePoint + sectorDisplacement;
            builders.Add(CreateClippedCone(model, startConePoint, endConePoint, minConeDiameter, maxConeDiameter));

            FixedVector<Dim3> startExtrudedAreaSolidPoint = endConePoint;
            FixedVector<Dim3> endExtrudedAreaSolidPoint = startExtrudedAreaSolidPoint + sectorDisplacement;
            builders.Add(CreateExtrudedAreaSolid(model, startExtrudedAreaSolidPoint, endExtrudedAreaSolidPoint, extrudedAreaSolidDiameter));

            return new VesselGeometry(builders);
        }

        private static IIfcBuilder CreateClippedCone(IModel model, FixedVector<Dim3> start, FixedVector<Dim3> end, double startDiameter, double endDiameter)
        {
            IfcTriangulatedProperties properties = IfcTriangulatedProperties.CreateClippedCone(new ClippedConeTriangulatedGeometryProperties
            {
                BottomConeCenter = start,
                TopConeCenter = end,

                BottomDiameter = startDiameter,
                TopDiameter = endDiameter
            });

            IIfcTriangulatedFaceSetBuilder<IIfcTriangulatedFaceSet> builder = new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
            builder.CreateCoordinates(model, properties.Coordinates);
            builder.AssignTriangleIndices(properties.TriangleIndices);
            builder.AssignNormals(properties.Normals);

            return builder;
        }

        private static IIfcBuilder CreateExtrudedAreaSolid(IModel model, FixedVector<Dim3> start, FixedVector<Dim3> end, double diameter)
        {
            FixedVector<Dim3> projection = end - start;
            double length = projection.L2Norm();
            FixedVector<Dim3> direction = projection * (1 / length);

            FixedVector<Dim3> zAxis = direction;
            FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
            FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

            FixedMatrix<Dim4> profileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
            FixedMatrix<Dim4> extrudedAreaSolidMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(start, xAxis, yAxis, zAxis);

            IIfcCircleProfileDefBuilder<IIfcCircleProfileDef> profileDefBuilder =
                new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(diameter / 2, IfcProfileTypeEnum.AREA, "");
            profileDefBuilder.CreatePosition(model, profileDefMatrix);

            IIfcProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

            IIfcExtrudedAreaSolidBuilder<IIfcExtrudedAreaSolid> builder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(length, FixedVector<Dim3>.Builder.Z(), profileDef);
            builder.CreatePosition(model, extrudedAreaSolidMatrix);

            return builder;
        }
    }
}