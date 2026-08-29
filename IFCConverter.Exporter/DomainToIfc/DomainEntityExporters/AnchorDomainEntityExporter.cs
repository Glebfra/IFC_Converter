using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class AnchorDomainEntityExporter : IDomainEntityExporter
    {
        private readonly IAnchorDomainEntityExportersRegistry _registry = new AnchorDomainEntityExportersRegistry();
        
        public bool CanExport(Entity entity)
        {
            return entity is Anchor;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Anchor anchor = (Anchor)entity;
            if (_registry.TryResolve(anchor, out IAnchorDomainEntityExporter exporter))
                exporter.Export(anchor, model, context);
        }
    }
}