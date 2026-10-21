using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Attributes;
using IFCConverter.IFC.Builders.Geometry.ProfileDef;
using IFCConverter.IFC.Builders.Geometry.SolidModel;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.IFC.Interfaces.Geometry.ProfileDef;
using IFCConverter.IFC.Interfaces.Geometry.SolidModel;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;

namespace IFCConverter.Exporter.Geometries
{
    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.SweptSolid)]
    internal sealed class AxialCouplingJointIfcGeometry : IfcGeometry
    {
        private const double DiameterToLengthFactor = 0.1;
        private const double DiameterToOuterDiameterFactor = 1.1;
        
        public AxialCouplingJointIfcGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null) 
            : base(geometryBuilder, representationContext)
        {
        }

        public AxialCouplingJointIfcGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null) 
            : base(geometryBuilders, representationContext)
        {
        }

        public static AxialCouplingJointIfcGeometry Create(IModel model, Joint joint)
        {
            FixedVector<Dim3> direction = joint.PortA.Direction.Negate();
            double diameter = joint.Ports.Max(port => port.Metadata.Diameter);
            
            double length = diameter * DiameterToLengthFactor;
            double innerDiameter = diameter;
            double outerDiameter = diameter * DiameterToOuterDiameterFactor;
            double wallThickness = outerDiameter - innerDiameter;

            FixedVector<Dim3> extrudedAreaPoint = direction * (-length * 0.5);

            FixedVector<Dim3> zAxis = direction.SafeNormalize();
            FixedVector<Dim3> xAxis = zAxis.CreateNormalVector().SafeNormalize();
            FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis).SafeNormalize();

            FixedMatrix<Dim4> profileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
            FixedMatrix<Dim4> extrudedAreaMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(extrudedAreaPoint, xAxis, yAxis, zAxis);

            IIfcCircleHollowProfileDefBuilder<IfcCircleHollowProfileDef> profileDefBuilder =
                new IfcCircleHollowProfileDefBuilder<IfcCircleHollowProfileDef>(
                    outerDiameter / 2, wallThickness / 2, IfcProfileTypeEnum.AREA,
                    $"{nameof(SphericalPipesJointGeometry)} {nameof(IfcCircleProfileDef)}"
                );
            profileDefBuilder.CreatePosition(model, profileDefMatrix);
            IfcCircleHollowProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(length, FixedVector<Dim3>.Builder.Z(), profileDef);
            extrudedAreaSolidBuilder.CreatePosition(model, extrudedAreaMatrix);

            return new AxialCouplingJointIfcGeometry(extrudedAreaSolidBuilder);
        }
    }
}