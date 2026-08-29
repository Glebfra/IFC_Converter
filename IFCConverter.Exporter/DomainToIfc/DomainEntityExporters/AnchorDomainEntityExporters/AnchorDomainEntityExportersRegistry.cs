using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal sealed class AnchorDomainEntityExportersRegistry : ReflectionRegistry<IAnchorDomainEntityExporter>, IAnchorDomainEntityExportersRegistry
    {
        public AnchorDomainEntityExportersRegistry() : base(typeof(AnchorDomainEntityExportersRegistry).Assembly)
        {
        }

        public IAnchorDomainEntityExporter Resolve(Anchor anchor)
        {
            return Resolve(exporter => exporter.CanExport(anchor));
        }

        public bool TryResolve(Anchor anchor, out IAnchorDomainEntityExporter exporter)
        {
            return TryResolve(exp => @exp.CanExport(anchor), out exporter);
        }
    }
}