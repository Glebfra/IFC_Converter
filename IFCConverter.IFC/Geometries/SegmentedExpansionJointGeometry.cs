using System.Collections.Generic;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Attributes;
using IFCConverter.IFC.Builders.Geometry.ProfileDef;
using IFCConverter.IFC.Builders.Geometry.SolidModel;
using IFCConverter.IFC.Interfaces;
using IFCConverter.IFC.Interfaces.Geometry.ProfileDef;
using IFCConverter.IFC.Interfaces.Geometry.SolidModel;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.IFC.Geometries
{
    public struct SegmentedExpansionJointGeometryProperties
    {
        public Vector<double> Position;
        public Vector<double>[] Points;
        public double Diameter;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    public class SegmentedExpansionJointGeometry : IfcGeometry
    {
        public SegmentedExpansionJointGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public SegmentedExpansionJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static SegmentedExpansionJointGeometry CreateGeometry(IModel model,
            SegmentedExpansionJointGeometryProperties properties)
        {
            double length = (properties.Points[1] - properties.Points[0]).L2Norm();
            Vector<double> direction = properties.Position - properties.Points[0];
            Vector<double> extrudedPoint = properties.Points[0];

            Matrix<double> extrudedMatrix = MatrixExtensions.CreateTransition(extrudedPoint, direction);
            Matrix<double> profileDefMatrix =
                MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);

            IIfcCircleProfileDefBuilder<IfcCircleProfileDef> profileDefBuilder =
                new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                    properties.Diameter / 2, IfcProfileTypeEnum.AREA,
                    $"{nameof(SegmentedExpansionJointGeometry)} {nameof(IfcCircleProfileDef)}"
                );
            profileDefBuilder.CreatePosition(model, profileDefMatrix);
            IfcCircleProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(length, VectorExtensions.Z, profileDef);
            extrudedAreaSolidBuilder.CreatePosition(model, extrudedMatrix);

            return new SegmentedExpansionJointGeometry(extrudedAreaSolidBuilder);
        }
    }
}