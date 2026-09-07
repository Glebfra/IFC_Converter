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
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.IFC.Geometries
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