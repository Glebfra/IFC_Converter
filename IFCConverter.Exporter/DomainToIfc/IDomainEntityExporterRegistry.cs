using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.DomainToIfc.DomainEntityExporters;
using IFCConverter.Utils.Registries;

namespace IFCConverter.Exporter.DomainToIfc
{
    internal interface IDomainEntityExporterRegistry : IRegistry<Entity, IDomainEntityExporter>
    {
    }
}