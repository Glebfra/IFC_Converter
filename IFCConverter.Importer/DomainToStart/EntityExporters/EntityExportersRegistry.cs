using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class EntityExportersRegistry : ReflectionRegistry<IEntityExporter>, IEntityExportersRegistry
    {
        public EntityExportersRegistry() : base(typeof(EntityExportersRegistry).Assembly)
        {
        }

        public IEntityExporter Resolve(Entity entity)
        {
            return Resolve(exporter => exporter.CanExport(entity));
        }

        public bool TryResolve(Entity entity, out IEntityExporter exporter)
        {
            return TryResolve(exp => exp.CanExport(entity), out exporter);
        }
    }
}