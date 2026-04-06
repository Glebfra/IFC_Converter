using System.Collections.Generic;
using System.Linq;
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
    public struct DoubleExtrudedJointGeometryProperties
    {
        public Vector<double> Position;
        public Vector<double>[] Points;
        public double Diameter;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.Brep)]
    public class AxialExpansionJointGeometry : IfcGeometry
    {
        private const double DiameterToFirstDiameterFactor = 1.0;
        private const double DiameterToSecondDiameterFactor = 0.75;

        public AxialExpansionJointGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public AxialExpansionJointGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        public static AxialExpansionJointGeometry CreateGeometry(IModel model,
            DoubleExtrudedJointGeometryProperties properties)
        {
            List<IIfcBuilder> builders = new();

            double length = (properties.Points[1] - properties.Points[0]).L2Norm();
            Vector<double>[] directions = properties.Points
                .Select(point => point - properties.Position)
                .ToArray();
            double[] diameters =
            {
                properties.Diameter * DiameterToFirstDiameterFactor,
                properties.Diameter * DiameterToSecondDiameterFactor
            };

            for (int i = 0; i < directions.Length; i++)
            {
                Vector<double> direction = directions[i];
                Matrix<double> extrudedAreaMatrix = MatrixExtensions.CreateTransition(properties.Position, direction);
                Matrix<double> profileDefMatrix =
                    MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Z);

                IIfcCircleProfileDefBuilder<IfcCircleProfileDef> profileDefBuilder =
                    new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(
                        diameters[i] / 2, IfcProfileTypeEnum.AREA,
                        $"{nameof(AxialExpansionJointGeometry)} {nameof(IfcCircleProfileDef)}"
                    );
                profileDefBuilder.CreatePosition(model, profileDefMatrix);
                IfcCircleProfileDef profileDef = profileDefBuilder.CreateProfileDef(model);

                IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid> extrudedAreaSolidBuilder =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(length / 2, VectorExtensions.Z, profileDef);
                extrudedAreaSolidBuilder.CreatePosition(model, extrudedAreaMatrix);

                builders.Add(extrudedAreaSolidBuilder);
            }

            return new AxialExpansionJointGeometry(builders);
        }
    }
}