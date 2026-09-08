using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.DomainToIfc.DomainEntityExporters;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.DomainToIfc
{
    internal sealed class DomainEntityExporterRegistry : ReflectionRegistry<IDomainEntityExporter>, IDomainEntityExporterRegistry
    {
        public DomainEntityExporterRegistry() : base(typeof(DomainEntityExporterRegistry).Assembly)
        {
        }

        public IDomainEntityExporter Resolve(Entity entity)
        {
            return Resolve(exporter => exporter.CanExport(entity));
        }

        public IEnumerable<IDomainEntityExporter> ResolveAll(Entity source)
        {
            return ResolveAll(exporter => exporter.CanExport(source));
        }

        public bool TryResolve(Entity entity, out IDomainEntityExporter exporter)
        {
            return TryResolve(exp => exp.CanExport(entity), out exporter);
        }
    }
}