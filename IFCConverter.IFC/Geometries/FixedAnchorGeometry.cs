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
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public FixedAnchorGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
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