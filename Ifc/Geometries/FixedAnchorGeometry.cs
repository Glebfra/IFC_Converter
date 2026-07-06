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
    public struct FixedAnchorGeometryProperties
    {
        public Vector<double> Position;
        public Vector<double> Direction;
        public double Diameter;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    public class FixedAnchorGeometry : IfcGeometry
    {
        private const double DiameterToLengthFactor = 0.1;
        private const double DiameterToXDimFactor = 1.5;
        private const double XDimToYDimFactor = 1;

        public FixedAnchorGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public FixedAnchorGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static FixedAnchorGeometry CreateGeometry(IModel model,
            FixedAnchorGeometryProperties properties)
        {
            double length = properties.Diameter * DiameterToLengthFactor;

            Vector<double> extrudedPoint = properties.Position - properties.Direction.Normalize(2) * length;
            Matrix<double> extrudedAreaMatrix =
                MatrixExtensions.CreateTransition(extrudedPoint, properties.Direction);
            Matrix<double> profileDefMatrix =
                MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);

            double xDim = properties.Diameter * DiameterToXDimFactor;
            double yDim = xDim * XDimToYDimFactor;
            IIfcRectangleProfileDefBuilder<IfcRectangleProfileDef> rectangleProfileDefBuilder =
                new IfcRectangleProfileDefBuilder<IfcRectangleProfileDef>(
                    xDim, yDim, IfcProfileTypeEnum.AREA,
                    $"{nameof(FixedAnchorGeometry)} {nameof(IfcRectangleProfileDef)}"
                );
            rectangleProfileDefBuilder.CreatePosition(model, profileDefMatrix);
            IfcRectangleProfileDef profileDef = rectangleProfileDefBuilder.CreateProfileDef(model);

            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                    length, VectorExtensions.Z, profileDef
                );
            extrudedAreaSolidBuilder.CreatePosition(model, extrudedAreaMatrix);

            return new FixedAnchorGeometry(extrudedAreaSolidBuilder);
        }
    }
}