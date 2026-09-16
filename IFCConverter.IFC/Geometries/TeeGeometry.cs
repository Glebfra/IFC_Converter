using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
    public struct TeeGeometryProperties
    {
        public double HeadLength;
        public double HeadDiameter;
        public FixedVector<Dim3> HeadDirection;

        public double MainLength;
        public double MainDiameter;
        public FixedVector<Dim3> MainDirection;

        public FixedVector<Dim3> Position;
    }

    [IfcRepresentationIdentifier(IfcRepresentationIdentifier.Body)]
    [IfcRepresentationType(IfcRepresentationType.SweptSolid)]
    public class TeeGeometry : IfcGeometry
    {
        public TeeGeometry(IIfcBuilder geometryBuilder,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilder, representationContext)
        {
        }

        public TeeGeometry(IEnumerable<IIfcBuilder> geometryBuilders,
            IIfcRepresentationContext representationContext = null)
            : base(geometryBuilders, representationContext)
        {
        }

        [Pure]
        public static TeeGeometry CreateGeometry(IModel model, TeeGeometryProperties properties)
        {
            double[] diameters =
            {
                properties.MainDiameter, properties.HeadDiameter
            };
            double[] lengths =
            {
                properties.MainLength, properties.HeadLength
            };

            FixedVector<Dim3>[] zs =
            {
                properties.MainDirection, properties.HeadDirection
            };
            FixedVector<Dim3>[] xs = zs.Select(z => z.CreateNormalVector()).ToArray();
            FixedVector<Dim3>[] ys = zs.Select((z, index) => z.CreateNormalVector(xs[index]).Normalize()).ToArray();

            FixedVector<Dim3>[] positions =
            {
                properties.Position - properties.MainDirection * (properties.MainLength / 2), properties.Position
            };

            FixedMatrix<Dim4> circleProfileDefMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros());
            FixedMatrix<Dim4>[] extrudedAreaSolidMatrices = positions
                .Select((pos, index) => FixedMatrix<Dim4>.Builder.CreateTransition(pos, xs[index], ys[index], zs[index]))
                .ToArray();

            IIfcCircleProfileDefBuilder<IfcCircleProfileDef>[] profileDefBuilders =
                new IIfcCircleProfileDefBuilder<IfcCircleProfileDef>[2];
            IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>[] extrudedAreaSolidBuilders =
                new IIfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>[2];

            for (int i = 0; i < 2; i++)
            {
                profileDefBuilders[i] =
                    new IfcCircleProfileDefBuilder<IfcCircleProfileDef>(diameters[i] / 2, IfcProfileTypeEnum.AREA, "Tee profile def");
                profileDefBuilders[i].CreatePosition(model, circleProfileDefMatrix);
                IIfcCircleProfileDef circleProfileDef = profileDefBuilders[i].CreateProfileDef(model);

                extrudedAreaSolidBuilders[i] =
                    new IfcExtrudedAreaSolidBuilder<IfcExtrudedAreaSolid>(
                        lengths[i], FixedVector<Dim3>.Builder.Z(), circleProfileDef
                    );
                extrudedAreaSolidBuilders[i].CreatePosition(model, extrudedAreaSolidMatrices[i]);
            }

            return new TeeGeometry(extrudedAreaSolidBuilders);
        }
    }
}