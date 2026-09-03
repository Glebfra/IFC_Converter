using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.EquipmentDomainEntityExporters
{
    internal sealed class EquipmentDomainEntityExportersRegistry : ReflectionRegistry<IEquipmentDomainEntityExporter>, IEquipmentDomainEntityExportersRegistry
    {
        public EquipmentDomainEntityExportersRegistry() : base(typeof(EquipmentDomainEntityExportersRegistry).Assembly)
        {
        }

        public IEquipmentDomainEntityExporter Resolve(Equipment equipment)
        {
            return Resolve(exporter => exporter.CanExport(equipment));
        }

        public bool TryResolve(Equipment equipment, out IEquipmentDomainEntityExporter exporter)
        {
            return TryResolve(exp => exp.CanExport(equipment), out exporter);
        }
    }
}