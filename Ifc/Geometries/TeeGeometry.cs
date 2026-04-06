using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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
using Xbim.Ifc4.ProfileResource;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace Ifc.Geometries
{
    public struct TeeGeometryProperties
    {
        public double HeadLength;
        public double HeadDiameter;
        public Vector<double> HeadDirection;

        public double MainLength;
        public double MainDiameter;
        public Vector<double> MainDirection;

        public Vector<double> Position;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.SolidModel)]
    public class TeeGeometry : IfcGeometry
    {
        public TeeGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public TeeGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext? representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        [Pure]
        public static TeeGeometry CreateGeometry(IModel model, TeeGeometryProperties properties)
        {
            double[] diameters = { properties.MainDiameter, properties.HeadDiameter };
            double[] lengths = { properties.MainLength, properties.HeadLength };

            Vector<double>[] zs = { properties.MainDirection, properties.HeadDirection };
            Vector<double>[] xs = zs.Select(z => z.CreateNormalVector()).ToArray();
            Vector<double>[] ys = zs.Select((z, index) => z.CrossProduct(xs[index]).Normalize(2)).ToArray();

            Vector<double>[] positions =
            {
                properties.Position - properties.MainDirection * properties.MainLength / 2,
                properties.Position
            };

            Matrix<double>[] extrudedAreaSolidMatrices = positions
                .Select((pos, index) => MatrixExtensions.CreateTransition(pos, xs[index], ys[index], zs[index]))
                .ToArray();
            Matrix<double> circleProfileDefMatrix =
                MatrixExtensions.CreateTransition(VectorExtensions.Zero, VectorExtensions.Right);

            IIfcCircleProfileDefBuilder<IfcCircleProfileDef>[] profileDefBuilders =
                new IIfcCircleProfileDefBuilder<IfcCircleProfileDef>[2];
            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>[] extrudedAreaSolidBuilders =
                new IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>[2];

            for (int i = 0; i < 2; i++)
            {
                profileDefBuilders[i] =
                    new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(diameters[i] / 2, IfcProfileTypeEnum.AREA,
                        "Test profile def");
                profileDefBuilders[i].CreatePosition(model, circleProfileDefMatrix);
                IIfcCircleProfileDef circleProfileDef = profileDefBuilders[i].CreateProfileDef(model);

                extrudedAreaSolidBuilders[i] =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                        lengths[i], VectorExtensions.Z, circleProfileDef
                    );
                extrudedAreaSolidBuilders[i].CreatePosition(model, extrudedAreaSolidMatrices[i]);
            }

            return new TeeGeometry(extrudedAreaSolidBuilders);
        }
    }
}