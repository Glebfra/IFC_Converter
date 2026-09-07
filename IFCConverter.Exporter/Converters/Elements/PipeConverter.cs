using System;
using System.Diagnostics.Contracts;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Utils.Diagnostics;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Segments;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class PipeConverter : IfcElementConverter<StartAbstractSegmentEntity, IfcPipeSegment>
    {
        private readonly Logger _logger = Logger.GetInstance();

        public PipeConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartAbstractSegmentEntity start)
        {
            IIfcGeometry pipeGeometry = PipeGeometry.CreateGeometry(_Model, new PipeGeometryProperties
            {
                Diameter = start.Diameter.SIProperty,
                Length = start.Length,
                Position = VectorExtensions.Zero,
                Direction = start.TransformationMatrix.GetForward()
            });
            pipeGeometry.AssignColor(GetIfcColor(start));
            return pipeGeometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartAbstractSegmentEntity start)
        {
            return MatrixExtensions.CreateTransition(start.TransformationMatrix.GetOffset());
        }

        public override IIfcProductBuilder<IfcPipeSegment> CreateBuilder(StartAbstractSegmentEntity start)
        {
            return new IfcPipeSegmentBuilder<IfcPipeSegment>(GenerateName(start), GenerateTag(start),
                GetIfcTypeEnum(start));
        }

        public override StartAbstractSegmentEntity BuildStartElement(IfcPipeSegment ifc)
        {
            throw new NotImplementedException();
        }

        [Pure]
        private static IfcPipeSegmentTypeEnum GetIfcTypeEnum(StartAbstractSegmentEntity start)
        {
            switch (start)
            {
                case StartFlexibleElementEntity startFlexibleElementEntity:
                    return IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
                case StartRigidElementEntity _:
                case StartCylindricalShellEntity _:
                case StartConeElementEntity _:
                case StartPipeEntity _:
                    return IfcPipeSegmentTypeEnum.RIGIDSEGMENT;
                default:
                    return IfcPipeSegmentTypeEnum.USERDEFINED;
            }
        }

        [Pure]
        private static Color GetIfcColor(StartAbstractSegmentEntity start)
        {
            switch (start)
            {
                case StartFlexibleElementEntity startFlexibleElementEntity:
                    return Color.FromHEX("00509f");
                case StartRigidElementEntity startRigidElementEntity:
                    return Color.FromHEX("009249");
                case StartCylindricalShellEntity startCylindricalShellEntity:
                    return Color.FromHEX("3e3ec0");
                case StartConeElementEntity startConeElementEntity:
                    return Color.FromHEX("46008b");
                default:
                    return Color.FromHEX("bebebe");
            }
        }
    }
}