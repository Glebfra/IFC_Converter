using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ifc.API;
using Ifc.Attributes;
using Ifc.Builders.Geometry.ProfileDef;
using Ifc.Builders.Geometry.SolidModel;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace Ifc.Geometries
{
    public struct BendGeometryProperties
    {
        public double BendRadius;
        public double PipeDiameter;

        public Vector<double> Position;
        public Vector<double> Direction;
        public Vector<double> EndDirection;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.SolidModel)]
    public class BendGeometry : IfcGeometry
    {
        public BendGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public BendGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        [Pure]
        public static BendGeometry CreateGeometry(IModel model, BendGeometryProperties properties)
        {
            double angle = properties.Direction.Angle(properties.EndDirection);
            Vector<double> z = properties.Direction.Normalize(2);
            Vector<double> y = properties.Direction.CrossProduct(properties.EndDirection).Normalize(2);
            Vector<double> x = y.CrossProduct(z).Normalize(2);

            Vector<double> axisPosition = VectorExtensions.X * properties.BendRadius;

            Matrix<double> circleProfileDefMatrix = MatrixExtensions.CreateTransition(VectorExtensions.Zero);
            Matrix<double> revolvedAreaSolidMatrix = MatrixExtensions.CreateTransition(properties.Position, x, y, z);

            IIfcCircleProfileDefBuilder<IIfcCircleProfileDef> circleProfileDefBuilder =
                new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                    properties.PipeDiameter / 2,
                    IfcProfileTypeEnum.AREA, "Test profile def"
                );
            circleProfileDefBuilder.CreatePosition(model, circleProfileDefMatrix);
            IIfcCircleProfileDef profileDef = circleProfileDefBuilder.CreateProfileDef(model);

            IIfcRevolvedAreaSolidBuilder<IIfcRevolvedAreaSolid> revolvedAreaSolidBuilder =
                new IfcRevolvedAreaSolidBuilder<IfcRevolvedAreaSolid>(angle, profileDef);
            revolvedAreaSolidBuilder.CreateAxis(model, axisPosition, VectorExtensions.Y.Negate());
            revolvedAreaSolidBuilder.CreatePosition(model, revolvedAreaSolidMatrix);

            return new BendGeometry(revolvedAreaSolidBuilder);
        }
    }
}