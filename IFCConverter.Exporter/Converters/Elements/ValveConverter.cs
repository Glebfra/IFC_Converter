using System;
using System.Linq;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using IFCConverter.Exporter.Extensions;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class ValveConverter : IfcElementConverter<StartValveEntity, IfcPipeFitting>
    {
        private readonly Logger _logger = Logger.GetInstance();

        public ValveConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartValveEntity start)
        {
            Vector<double> globalTopConePoint = start.Position;
            Vector<double>[] globalBotConePoints = start.GetBotConePoints();

            Vector<double> localTopConePoint = VectorExtensions.Zero;
            Vector<double>[] localBotConePoints = globalBotConePoints
                .Select(point => point - globalTopConePoint)
                .ToArray();

            IIfcGeometry valveGeometry = ValveGeometry.CreateGeometry(_Model, new ValveGeometryProperties
            {
                Diameter = start.OutsideDiameter.SIProperty,
                Length = start.Length.SIProperty,
                TopConePoint = localTopConePoint,
                BotConePoints = localBotConePoints
            });
            valveGeometry.AssignColor(Color.FromHEX("5f4e7c"));
            return valveGeometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartValveEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcPipeFitting> CreateBuilder(StartValveEntity start)
        {
            return new IfcPipeFittingBuilder<IfcPipeFitting>(GenerateName(start), GenerateTag(start),
                IfcPipeFittingTypeEnum.OBSTRUCTION);
        }

        public override StartValveEntity BuildStartElement(IfcPipeFitting ifc)
        {
            throw new NotImplementedException();
        }
    }
}