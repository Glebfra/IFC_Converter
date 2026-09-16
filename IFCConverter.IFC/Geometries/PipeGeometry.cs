using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;

namespace IFCConverter.IFC.Geometries
{
    public struct PipeGeometryProperties
    {
        public double Length;
        public double Diameter;
        public FixedVector<Dim3> Position;
        public FixedVector<Dim3> Direction;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.SolidModel)]
    public class PipeGeometry : IfcGeometry
    {
        public PipeGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public PipeGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        [Pure]
        public static PipeGeometry CreateGeometry(IModel model, PipeGeometryProperties properties)
        {
            FixedVector<Dim3> zAxis = properties.Direction;
            FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
            FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis).Normalize();

            FixedMatrix<Dim4> circleProfileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
            FixedMatrix<Dim4> extrudedAreaSolidMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(properties.Position, xAxis, yAxis, zAxis);

            double circleProfileDefRadius = properties.Diameter / 2;
            IIfcCircleProfileDefBuilder<IfcCircleProfileDef> circleProfileDefBuilder =
                new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                    circleProfileDefRadius, IfcProfileTypeEnum.AREA, new IfcLabel("")
                );
            circleProfileDefBuilder.CreatePosition(model, circleProfileDefMatrix);
            IIfcCircleProfileDef circleProfileDef = circleProfileDefBuilder.CreateProfileDef(model);

            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                    properties.Length, FixedVector<Dim3>.Builder.Z(), circleProfileDef
                );
            extrudedAreaSolidBuilder.CreatePosition(model, extrudedAreaSolidMatrix);

            return new PipeGeometry(extrudedAreaSolidBuilder);
        }
    }
}