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
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace Ifc.Geometries
{
    public struct PipeGeometryProperties
    {
        public double Length;
        public double Diameter;
        public Vector<double> Position;
        public Vector<double> Direction;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.SolidModel)]
    public class PipeGeometry : IfcGeometry
    {
        public PipeGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public PipeGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        [Pure]
        public static PipeGeometry CreateGeometry(IModel model, PipeGeometryProperties properties)
        {
            Vector<double> z = properties.Direction;
            Vector<double> x = z.CreateNormalVector();
            Vector<double> y = z.CrossProduct(x).Normalize(2);

            Matrix<double> extrudedAreaSolidMatrix = MatrixExtensions.CreateTransition(properties.Position, x, y, z);
            Matrix<double> circleProfileDefMatrix =
                MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);

            double circleProfileDefRadius = properties.Diameter / 2;
            IIfcCircleProfileDefBuilder<IfcCircleProfileDef> circleProfileDefBuilder =
                new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                    circleProfileDefRadius, IfcProfileTypeEnum.AREA, new IfcLabel("")
                );
            circleProfileDefBuilder.CreatePosition(model, circleProfileDefMatrix);
            IIfcCircleProfileDef circleProfileDef = circleProfileDefBuilder.CreateProfileDef(model);

            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                    properties.Length, VectorExtensions.Forward, circleProfileDef
                );
            extrudedAreaSolidBuilder.CreatePosition(model, extrudedAreaSolidMatrix);

            return new PipeGeometry(extrudedAreaSolidBuilder);
        }
    }
}