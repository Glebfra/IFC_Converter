using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.EquipmentDomainEntityExporters;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class EquipmentDomainEntityExporter : IDomainEntityExporter
    {
        private readonly IEquipmentDomainEntityExportersRegistry _registry = new EquipmentDomainEntityExportersRegistry();
        
        public bool CanExport(Entity entity)
        {
            return entity is Equipment;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Equipment equipment = (Equipment)entity;
            if (_registry.TryResolve(equipment, out IEquipmentDomainEntityExporter exporter))
                exporter.Export(equipment, model, context);
        }
    }
}