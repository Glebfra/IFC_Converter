using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Geometries;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

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

            IIfcGeometry geometry = ValveIfcGeometry.Create(model, valve);
            geometry.AssignColor(Color.FromHEX(valve.Metadata.Color));
            
            FixedMatrix<Dim4> placement = FixedMatrix<Dim4>.Builder.CreateTransition(valve.Position);
            IIfcPipeFittingBuilder<IIfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(valve.Metadata.Name, valve.Metadata.Type, IfcPipeFittingTypeEnum.CONNECTOR);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);
            builder.AssignObjectType(entity.Metadata.Type);

            IIfcPipeFitting instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}