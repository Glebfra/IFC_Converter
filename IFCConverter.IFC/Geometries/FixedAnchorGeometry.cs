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
    public struct FixedAnchorGeometryProperties
    {
        public FixedVector<Dim3> Position;
        public FixedVector<Dim3> Direction;
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

            FixedVector<Dim3> extrudedPoint = properties.Position - properties.Direction.Normalize() * length;
            FixedMatrix<Dim4> profileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());

            FixedVector<Dim3> zAxis = properties.Direction.Normalize();
            FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
            FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);
            FixedMatrix<Dim4> extrudedAreaMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(extrudedPoint, xAxis, yAxis, zAxis);

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
                    length, FixedVector<Dim3>.Zeros(), profileDef
                );
            extrudedAreaSolidBuilder.CreatePosition(model, extrudedAreaMatrix);

            return new FixedAnchorGeometry(extrudedAreaSolidBuilder);
        }
    }
}