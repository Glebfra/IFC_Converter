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
    public struct VesselGeometryProperties
    {
        public Vector<double>[] Points { get; set; }
        public double Diameter { get; set; }
    }
    
    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Tessellation)]
    public sealed class VesselGeometry : IfcGeometry
    {
        private const double GeometrySectorLengthFactor = 0.5;
        private const double ConeMaxDiameterFactor = 1.1;
        private const double DiameterExtrudedAreaSolidFactor = 1.3;
        
        public VesselGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null) : base(geometryBuilder, representationContext)
        {
        }

        public VesselGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null) : base(geometryBuilders, representationContext)
        {
        }

        public static VesselGeometry CreateGeometry(IModel model, VesselGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();
            
            Vector<double> projection = properties.Points[1] - properties.Points[0];
            double length = projection.L2Norm();
            Vector<double> direction = projection / length;
            Vector<double> sectorDisplacement = direction * length * GeometrySectorLengthFactor;

            double extrudedAreaSolidDiameter = properties.Diameter * DiameterExtrudedAreaSolidFactor;
            double minConeDiameter = properties.Diameter;
            double maxConeDiameter = minConeDiameter * ConeMaxDiameterFactor;
            
            Vector<double> startConePoint = properties.Points[0];
            Vector<double> endConePoint = startConePoint + sectorDisplacement;
            builders.Add(CreateClippedCone(model, startConePoint, endConePoint, minConeDiameter, maxConeDiameter));

            Vector<double> startExtrudedAreaSolidPoint = endConePoint;
            Vector<double> endExtrudedAreaSolidPoint = startExtrudedAreaSolidPoint + sectorDisplacement;
            builders.Add(CreateExtrudedAreaSolid(model, startExtrudedAreaSolidPoint, endExtrudedAreaSolidPoint, extrudedAreaSolidDiameter));

            return new VesselGeometry(builders);
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