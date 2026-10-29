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
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    internal sealed class AxialExpansionJointIfcGeometry : IfcGeometry
    {
        private const double DiameterToFirstDiameterFactor = 1.0;
        private const double DiameterToSecondDiameterFactor = 0.75;

        public AxialExpansionJointIfcGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public AxialExpansionJointIfcGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static AxialCouplingJointIfcGeometry Create(IModel model, Joint joint)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            double diameter = joint.Ports.Max(port => port.Metadata.Diameter);
            FixedVector<Dim3>[] points = joint.Ports.Select(port => port.Position - joint.Position).ToArray();

            double length = (points[1] - points[0]).L2Norm();
            double[] diameters =
            {
                diameter * DiameterToFirstDiameterFactor, diameter * DiameterToSecondDiameterFactor
            };

            for (int i = 0; i < points.Length; i++)
            {
                FixedVector<Dim3> direction = points[i];
                FixedVector<Dim3> zAxis = direction;
                FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
                FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

                FixedMatrix<Dim4> profileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
                FixedMatrix<Dim4> extrudedAreaMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros(), xAxis, yAxis, zAxis);

                IIfcCircleProfileDefBuilder<IfcCircleProfileDef> profileDefBuilder =
                    new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                        diameters[i] / 2, IfcProfileTypeEnum.AREA,
                        $"{nameof(AxialCouplingJointIfcGeometry)} {nameof(IfcCircleProfileDef)}"
                    );
                profileDefBuilder.CreatePosition(model, profileDefMatrix);
                IfcCircleProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(length / 2, FixedVector<Dim3>.Builder.Z(), profileDef);
                extrudedAreaSolidBuilder.CreatePosition(model, extrudedAreaMatrix);

                builders.Add(extrudedAreaSolidBuilder);
            }

            return new AxialCouplingJointIfcGeometry(builders);
        }
    }
}