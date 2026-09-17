using System.Collections.Generic;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Attributes;
using IFCConverter.IFC.Builders.Geometry.ProfileDef;
using IFCConverter.IFC.Builders.Geometry.SolidModel;
using IFCConverter.IFC.Interfaces;
using IFCConverter.IFC.Interfaces.Geometry.ProfileDef;
using IFCConverter.IFC.Interfaces.Geometry.SolidModel;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;

namespace IFCConverter.IFC.Geometries
{
    public struct SegmentedExpansionJointGeometryProperties
    {
        public FixedVector<Dim3> Position;
        public FixedVector<Dim3>[] Points;
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
            FixedVector<Dim3> direction = properties.Position - properties.Points[0];
            FixedVector<Dim3> extrudedPoint = properties.Points[0];

            FixedVector<Dim3> zAxis = direction;
            FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
            FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

            FixedMatrix<Dim4> profileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
            FixedMatrix<Dim4> extrudedMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(extrudedPoint, xAxis, yAxis, zAxis);

            IIfcCircleProfileDefBuilder<IfcCircleProfileDef> profileDefBuilder =
                new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                    properties.Diameter / 2, IfcProfileTypeEnum.AREA,
                    $"{nameof(SegmentedExpansionJointGeometry)} {nameof(IfcCircleProfileDef)}"
                );
            profileDefBuilder.CreatePosition(model, profileDefMatrix);
            IfcCircleProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(length, FixedVector<Dim3>.Builder.Z(), profileDef);
            extrudedAreaSolidBuilder.CreatePosition(model, extrudedMatrix);

            return new SegmentedExpansionJointGeometry(extrudedAreaSolidBuilder);
        }
    }
}