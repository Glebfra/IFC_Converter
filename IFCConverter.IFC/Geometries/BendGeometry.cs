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
using Xbim.Ifc4.ProfileResource;

namespace IFCConverter.IFC.Geometries
{
    public struct BendGeometryProperties
    {
        public double BendRadius;
        public double PipeDiameter;

        public FixedVector<Dim3> Position;
        public FixedVector<Dim3> Direction;
        public FixedVector<Dim3> EndDirection;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.SolidModel)]
    public class BendGeometry : IfcGeometry
    {
        public BendGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public BendGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        [Pure]
        public static BendGeometry CreateGeometry(IModel model, BendGeometryProperties properties)
        {
            double angle = properties.Direction.Angle(properties.EndDirection);
            FixedVector<Dim3> zAxis = properties.Direction.Normalize();
            FixedVector<Dim3> yAxis = properties.Direction.CrossProduct(properties.EndDirection).Normalize();
            FixedVector<Dim3> xAxis = yAxis.CreateNormalVector(zAxis).Normalize();

            FixedVector<Dim3> axisPosition = FixedVector<Dim3>.Builder.X() * properties.BendRadius;

            FixedMatrix<Dim4> circleProfileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
            FixedMatrix<Dim4> revolvedAreaSolidMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(properties.Position, xAxis, yAxis, zAxis);

            IIfcCircleProfileDefBuilder<IIfcCircleProfileDef> circleProfileDefBuilder =
                new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                    properties.PipeDiameter / 2,
                    IfcProfileTypeEnum.AREA, "Test profile def"
                );
            circleProfileDefBuilder.CreatePosition(model, circleProfileDefMatrix);
            IIfcCircleProfileDef profileDef = circleProfileDefBuilder.CreateProfileDef(model);

            IIfcRevolvedAreaSolidBuilder<IIfcRevolvedAreaSolid> revolvedAreaSolidBuilder =
                new IfcRevolvedAreaSolidBuilder<IfcRevolvedAreaSolid>(angle, profileDef);
            revolvedAreaSolidBuilder.CreateAxis(model, axisPosition, FixedVector<Dim3>.Builder.Y().Negate());
            revolvedAreaSolidBuilder.CreatePosition(model, revolvedAreaSolidMatrix);

            return new BendGeometry(revolvedAreaSolidBuilder);
        }
    }
}