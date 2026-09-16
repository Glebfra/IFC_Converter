using System.Collections.Generic;
using System.Linq;
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
    public struct DoubleExtrudedJointGeometryProperties
    {
        public FixedVector<Dim3> Position;
        public FixedVector<Dim3>[] Points;
        public double Diameter;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    public class AxialExpansionJointGeometry : IfcGeometry
    {
        private const double DiameterToFirstDiameterFactor = 1.0;
        private const double DiameterToSecondDiameterFactor = 0.75;

        public AxialExpansionJointGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public AxialExpansionJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static AxialExpansionJointGeometry CreateGeometry(IModel model,
            DoubleExtrudedJointGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new List<IIfcBuilder>();

            double length = (properties.Points[1] - properties.Points[0]).L2Norm();
            FixedVector<Dim3>[] directions = properties.Points
                .Select(point => point - properties.Position)
                .ToArray();
            double[] diameters =
            {
                properties.Diameter * DiameterToFirstDiameterFactor, properties.Diameter * DiameterToSecondDiameterFactor
            };

            for (int i = 0; i < directions.Length; i++)
            {
                FixedVector<Dim3> direction = directions[i];
                FixedVector<Dim3> zAxis = direction;
                FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
                FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

                FixedMatrix<Dim4> profileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
                FixedMatrix<Dim4> extrudedAreaMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(properties.Position, xAxis, yAxis, zAxis);

                IIfcCircleProfileDefBuilder<IfcCircleProfileDef> profileDefBuilder =
                    new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                        diameters[i] / 2, IfcProfileTypeEnum.AREA,
                        $"{nameof(AxialExpansionJointGeometry)} {nameof(IfcCircleProfileDef)}"
                    );
                profileDefBuilder.CreatePosition(model, profileDefMatrix);
                IfcCircleProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(length / 2, FixedVector<Dim3>.Builder.Z(), profileDef);
                extrudedAreaSolidBuilder.CreatePosition(model, extrudedAreaMatrix);

                builders.Add(extrudedAreaSolidBuilder);
            }

            return new AxialExpansionJointGeometry(builders);
        }
    }
}