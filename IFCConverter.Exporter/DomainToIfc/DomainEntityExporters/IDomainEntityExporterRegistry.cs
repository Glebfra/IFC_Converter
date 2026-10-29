using IFCConverter.Domain.Entities;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal interface IDomainEntityExporterRegistry
    {
        bool TryResolve(Entity entity, out IDomainEntityExporter exporter);
    }
}