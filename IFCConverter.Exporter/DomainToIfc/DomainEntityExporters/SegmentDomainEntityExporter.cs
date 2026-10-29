using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.SegmentDomainExporters;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class SegmentDomainEntityExporter : IDomainEntityExporter
    {
        private readonly ISegmentDomainEntityExportersRegistry _registry = new SegmentDomainEntityExportersRegistry();

        public bool CanExport(Entity entity)
        {
            return entity is Segment;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            if (_registry.TryResolve(entity, out ISegmentDomainEntityExporter exporter))
                exporter.Export(entity, model, context);
        }
    }
}