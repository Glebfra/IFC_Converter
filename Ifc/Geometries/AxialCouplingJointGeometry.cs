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
    public struct AxialCouplingJointGeometryProperties
    {
        public Vector<double> Position;
        public Vector<double> Direction;
        public double Diameter;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    public class AxialCouplingJointGeometry : IfcGeometry
    {
        private const double DiameterToLengthFactor = 0.1;
        private const double DiameterToOuterDiameterFactor = 1.1;

        public AxialCouplingJointGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public AxialCouplingJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static AxialCouplingJointGeometry CreateGeometry(IModel model,
            AxialCouplingJointGeometryProperties properties)
        {
            double length = properties.Diameter * DiameterToLengthFactor;
            double innerDiameter = properties.Diameter;
            double outerDiameter = properties.Diameter * DiameterToOuterDiameterFactor;
            double wallThickness = outerDiameter - innerDiameter;

            Vector<double> extrudedAreaPoint = properties.Position - properties.Direction * (length / 2);
            Matrix<double> extrudedAreaMatrix =
                MatrixExtensions.CreateTransition(extrudedAreaPoint, properties.Direction);
            Matrix<double> profileDefMatrix =
                MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);

            IIfcCircleHollowProfileDefBuilder<IfcCircleHollowProfileDef> profileDefBuilder =
                new IfcCircleHollowProfileDefBuilder<IfcCircleHollowProfileDef>(
                    outerDiameter / 2, wallThickness / 2, IfcProfileTypeEnum.AREA,
                    $"{nameof(SphericalPipesJointGeometry)} {nameof(IfcCircleProfileDef)}"
                );
            profileDefBuilder.CreatePosition(model, profileDefMatrix);
            IfcCircleHollowProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(length, VectorExtensions.Z, profileDef);
            extrudedAreaSolidBuilder.CreatePosition(model, extrudedAreaMatrix);

            return new AxialCouplingJointGeometry(extrudedAreaSolidBuilder);
        }
    }
}