using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Registries;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal interface IDomainEntityExporterRegistry : IRegistry<Entity, IDomainEntityExporter>
    {
    }
}