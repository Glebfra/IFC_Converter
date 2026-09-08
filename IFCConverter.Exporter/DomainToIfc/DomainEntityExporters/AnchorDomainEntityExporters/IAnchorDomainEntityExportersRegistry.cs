using IFCConverter.Domain.Entities;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal interface IAnchorDomainEntityExportersRegistry
    {
        IAnchorDomainEntityExporter Resolve(Anchor anchor);
        bool TryResolve(Anchor anchor, out IAnchorDomainEntityExporter exporter);
    }
}