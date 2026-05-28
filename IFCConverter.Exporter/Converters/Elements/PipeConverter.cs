using System;
using System.Diagnostics.Contracts;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Segments;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

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
            return start switch
            {
                StartFlexibleElementEntity => IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT,
                StartRigidElementEntity => IfcPipeSegmentTypeEnum.RIGIDSEGMENT,
                StartCylindricalShellEntity => IfcPipeSegmentTypeEnum.RIGIDSEGMENT,
                StartConeElementEntity => IfcPipeSegmentTypeEnum.RIGIDSEGMENT,
                StartPipeEntity => IfcPipeSegmentTypeEnum.RIGIDSEGMENT,
                _ => IfcPipeSegmentTypeEnum.USERDEFINED
            };
        }

        [Pure]
        private static Color GetIfcColor(StartAbstractSegmentEntity start)
        {
            return start switch
            {
                StartFlexibleElementEntity => Color.FromHEX("00509f"),
                StartRigidElementEntity => Color.FromHEX("009249"),
                StartCylindricalShellEntity => Color.FromHEX("3e3ec0"),
                StartConeElementEntity => Color.FromHEX("46008b"),
                _ => Color.FromHEX("bebebe")
            };
        }
    }
}