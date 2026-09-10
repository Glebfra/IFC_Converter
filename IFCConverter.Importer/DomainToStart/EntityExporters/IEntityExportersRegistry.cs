using IFCConverter.Domain.Entities;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal interface IEntityExportersRegistry
    {
        IEntityExporter Resolve(Entity entity);
        bool TryResolve(Entity entity, out IEntityExporter exporter);
    }
}