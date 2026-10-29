using IFCConverter.Domain.Entities;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal interface IAnchorDomainEntityExportersRegistry
    {
        bool TryResolve(Anchor anchor, out IAnchorDomainEntityExporter exporter);
    }
}