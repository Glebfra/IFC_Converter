using System;
using System.Linq;
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

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class ConeElementConverter : IfcElementConverter<StartConeElementEntity, IfcPipeSegment>
    {
        private readonly Logger _logger = Logger.GetInstance();

        public ConeElementConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartConeElementEntity start)
        {
            IIfcGeometry geometry = ConeGeometry.CreateGeometry(_Model, new ConeGeometryProperties
            {
                Positions = start.Points.ToArray(),
                Diameters = start.Diameters.ToArray()
            });
            geometry.AssignColor(Color.FromHEX("46008b"));
            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartConeElementEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcPipeSegment> CreateBuilder(StartConeElementEntity start)
        {
            return new IfcPipeSegmentBuilder<IfcPipeSegment>(GenerateName(start), GenerateTag(start),
                IfcPipeSegmentTypeEnum.USERDEFINED);
        }

        public override StartConeElementEntity BuildStartElement(IfcPipeSegment ifc)
        {
            throw new NotImplementedException();
        }
    }
}