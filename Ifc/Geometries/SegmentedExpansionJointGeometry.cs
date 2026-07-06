using System.Collections.Generic;
using Ifc.API;
using Ifc.Attributes;
using Ifc.Builders.Geometry.ProfileDef;
using Ifc.Builders.Geometry.SolidModel;
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
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public SegmentedExpansionJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
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