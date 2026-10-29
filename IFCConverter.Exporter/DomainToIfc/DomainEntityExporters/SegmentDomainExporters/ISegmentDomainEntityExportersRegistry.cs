using IFCConverter.Domain.Entities;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.SegmentDomainExporters
{
    internal interface ISegmentDomainEntityExportersRegistry
    {
        bool TryResolve(Entity entity, out ISegmentDomainEntityExporter exporter);
    }
}