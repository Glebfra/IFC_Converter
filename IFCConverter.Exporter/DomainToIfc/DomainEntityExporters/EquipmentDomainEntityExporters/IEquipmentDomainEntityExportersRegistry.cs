using IFCConverter.Domain.Entities;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.EquipmentDomainEntityExporters
{
    internal interface IEquipmentDomainEntityExportersRegistry
    {
        IEquipmentDomainEntityExporter Resolve(Equipment equipment);
        bool TryResolve(Equipment equipment, out IEquipmentDomainEntityExporter exporter);
    }
}