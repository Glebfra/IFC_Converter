using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class ValveDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Valve;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Valve valve = (Valve)entity;
            double diameter = valve.Ports.Max(port => port.Metadata.Diameter);
            Vector<double> valvePosition = valve.Position;

            Vector<double>[] botConePoints = valve.Ports.Select(port => valvePosition - port.Position).ToArray();

            IIfcGeometry geometry = ValveGeometry.CreateGeometry(model, new ValveGeometryProperties
            {
                Length = valve.Length,
                Diameter = diameter,
                BotConePoints = botConePoints,
                TopConePoint = VectorExtensions.Zero
            });
            geometry.AssignColor(Color.FromHEX(valve.Metadata.Color));

            Matrix<double> placement = MatrixExtensions.CreateTransition(valvePosition);
            IIfcPipeFittingBuilder<IIfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(valve.Metadata.Name, valve.Metadata.Type, IfcPipeFittingTypeEnum.CONNECTOR);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcPipeFitting instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}