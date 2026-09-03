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
    public struct PumpApi610GeometryProperties
    {
        public Vector<double>[] Points { get; set; }
        public double[] Diameters { get; set; }
    }
    
    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public sealed class PumpApi610Geometry : IfcGeometry
    {
        private const double ConeMaxDiameterFactor = 1.1;
        private const double GeometrySectorLengthFactor = 0.2;
        private const double DiameterExtrudedAreaSolidFactor = 1.3;
        
        public PumpApi610Geometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null) : base(geometryBuilder, representationContext)
        {
        }

        public PumpApi610Geometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null) : base(geometryBuilders, representationContext)
        {
        }

        public static PumpApi610Geometry CreateGeometry(IModel model, PumpApi610GeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            int pumpCount = properties.Points.Length / 2;
            for (int i = 0; i < pumpCount; i++)
            {
                int index = i * pumpCount;
                Vector<double> projection = properties.Points[index + 1] - properties.Points[index];
                double length = projection.L2Norm();
                Vector<double> direction = projection / length;
                Vector<double> sectorDisplacement = direction * length * GeometrySectorLengthFactor;

                double diameter = properties.Diameters[i];

                double extrudedAreaSolidDiameter = diameter * DiameterExtrudedAreaSolidFactor;
                double minConeDiameter = diameter;
                double maxConeDiameter = minConeDiameter * ConeMaxDiameterFactor;

                Vector<double> startFirstConePosition = properties.Points[index];
                Vector<double> endFirstConePosition = startFirstConePosition + sectorDisplacement;
                builders.Add(CreateClippedCone(model, startFirstConePosition, endFirstConePosition, minConeDiameter, maxConeDiameter));

                Vector<double> startFirstExtrudedAreaSolidPosition = endFirstConePosition;
                Vector<double> endFirstExtrudedAreaSolidPosition = startFirstExtrudedAreaSolidPosition + sectorDisplacement;
                builders.Add(CreateExtrudedAreaSolid(model, startFirstExtrudedAreaSolidPosition, endFirstExtrudedAreaSolidPosition, extrudedAreaSolidDiameter));

                Vector<double> startSkipPosition = endFirstExtrudedAreaSolidPosition;
                Vector<double> endSkipPosition = startSkipPosition + sectorDisplacement;

                Vector<double> startSecondExtrudedAreaSolidPosition = endSkipPosition;
                Vector<double> endSecondExtrudedAreaSolidPosition = startSecondExtrudedAreaSolidPosition + sectorDisplacement;
                builders.Add(CreateExtrudedAreaSolid(model, startSecondExtrudedAreaSolidPosition, endSecondExtrudedAreaSolidPosition, extrudedAreaSolidDiameter));

                Vector<double> startSecondConePosition = endSecondExtrudedAreaSolidPosition;
                Vector<double> endSecondConePosition = properties.Points[index + 1];
                builders.Add(CreateClippedCone(model, startSecondConePosition, endSecondConePosition, maxConeDiameter, minConeDiameter));
            }

            return new PumpApi610Geometry(builders);
        }

        private static IIfcBuilder CreateClippedCone(IModel model, Vector<double> start, Vector<double> end, double startDiameter, double endDiameter)
        {
            IfcTriangulatedProperties properties = IfcTriangulatedProperties.CreateClippedCone(new ClippedConeTriangulatedGeometryProperties()
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

        private static IIfcBuilder CreateExtrudedAreaSolid(IModel model, Vector<double> start, Vector<double> end, double diameter)
        {
            Vector<double> projection = (end - start);
            double length = projection.L2Norm();
            Vector<double> direction = projection / length;

            Matrix<double> profileDefMatrix = MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);
            IIfcCircleProfileDefBuilder<IIfcCircleProfileDef> profileDefBuilder =
                new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(diameter / 2, IfcProfileTypeEnum.AREA, "");
            profileDefBuilder.CreatePosition(model, profileDefMatrix);
            
            IIfcProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

            Matrix<double> extrudedAreaSolidMatrix = MatrixExtensions.CreateTransition(start, direction);
            IIfcExtrudedAreaSolidBuilder<IIfcExtrudedAreaSolid> builder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(length, VectorExtensions.Z, profileDef);
            builder.CreatePosition(model, extrudedAreaSolidMatrix);

            return builder;
        }
    }
}