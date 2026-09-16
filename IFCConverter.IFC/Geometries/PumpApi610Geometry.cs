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
    public struct PumpApi610GeometryProperties
    {
        public FixedVector<Dim3>[] Points { get; set; }
        public double[] Diameters { get; set; }
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public sealed class PumpApi610Geometry : IfcGeometry
    {
        private const double ConeMaxDiameterFactor = 1.1;
        private const double GeometrySectorLengthFactor = 0.2;
        private const double DiameterExtrudedAreaSolidFactor = 1.3;

        public PumpApi610Geometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null) : base(geometryBuilder,
            representationContext)
        {
        }

        public PumpApi610Geometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null) : base(
            geometryBuilders, representationContext)
        {
        }

        public static PumpApi610Geometry CreateGeometry(IModel model, PumpApi610GeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            int pumpCount = properties.Points.Length / 2;
            for (int i = 0; i < pumpCount; i++)
            {
                int index = i * pumpCount;
                FixedVector<Dim3> projection = properties.Points[index + 1] - properties.Points[index];
                double length = projection.L2Norm();
                FixedVector<Dim3> direction = projection * (1 / length);
                FixedVector<Dim3> sectorDisplacement = direction * length * GeometrySectorLengthFactor;

                double diameter = properties.Diameters[i];

                double extrudedAreaSolidDiameter = diameter * DiameterExtrudedAreaSolidFactor;
                double minConeDiameter = diameter;
                double maxConeDiameter = minConeDiameter * ConeMaxDiameterFactor;

                FixedVector<Dim3> startFirstConePosition = properties.Points[index];
                FixedVector<Dim3> endFirstConePosition = startFirstConePosition + sectorDisplacement;
                builders.Add(CreateClippedCone(model, startFirstConePosition, endFirstConePosition, minConeDiameter, maxConeDiameter));

                FixedVector<Dim3> startFirstExtrudedAreaSolidPosition = endFirstConePosition;
                FixedVector<Dim3> endFirstExtrudedAreaSolidPosition = startFirstExtrudedAreaSolidPosition + sectorDisplacement;
                builders.Add(CreateExtrudedAreaSolid(model, startFirstExtrudedAreaSolidPosition, endFirstExtrudedAreaSolidPosition,
                    extrudedAreaSolidDiameter));

                FixedVector<Dim3> startSkipPosition = endFirstExtrudedAreaSolidPosition;
                FixedVector<Dim3> endSkipPosition = startSkipPosition + sectorDisplacement;

                FixedVector<Dim3> startSecondExtrudedAreaSolidPosition = endSkipPosition;
                FixedVector<Dim3> endSecondExtrudedAreaSolidPosition = startSecondExtrudedAreaSolidPosition + sectorDisplacement;
                builders.Add(CreateExtrudedAreaSolid(model, startSecondExtrudedAreaSolidPosition, endSecondExtrudedAreaSolidPosition,
                    extrudedAreaSolidDiameter));

                FixedVector<Dim3> startSecondConePosition = endSecondExtrudedAreaSolidPosition;
                FixedVector<Dim3> endSecondConePosition = properties.Points[index + 1];
                builders.Add(CreateClippedCone(model, startSecondConePosition, endSecondConePosition, maxConeDiameter, minConeDiameter));
            }

            return new PumpApi610Geometry(builders);
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