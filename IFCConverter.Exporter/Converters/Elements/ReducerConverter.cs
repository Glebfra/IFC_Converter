using System;
using System.Linq;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Utils.Diagnostics;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class ReducerConverter : IfcElementConverter<StartAbstractReducerEntity, IfcPipeFitting>
    {
        private readonly Logger _logger = Logger.GetInstance();

        public ReducerConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartAbstractReducerEntity start)
        {
            Vector<double> forward = (
                start.Points.ElementAt(1) - start.Points.ElementAt(0)
            ).DotProduct(start.SegmentWithMinDiameter.Projection) * start.SegmentWithMinDiameter.Projection;
            Vector<double>[] positions = start.Points
                .Select(point => point - start.Position)
                .ToArray();

            IIfcGeometry geometry = ConeGeometry.CreateGeometry(_Model, new ConeGeometryProperties
            {
                Direction = forward,
                Positions = positions,
                Diameters = start.Diameters.ToArray()
            });
            geometry.AssignColor(Color.FromHEX("5f4e7c"));
            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartAbstractReducerEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcPipeFitting> CreateBuilder(StartAbstractReducerEntity start)
        {
            return new IfcPipeFittingBuilder<IfcPipeFitting>(GenerateName(start), GenerateTag(start),
                IfcPipeFittingTypeEnum.CONNECTOR);
        }

        public override StartAbstractReducerEntity BuildStartElement(IfcPipeFitting ifc)
        {
            throw new NotImplementedException();
        }
    }
}