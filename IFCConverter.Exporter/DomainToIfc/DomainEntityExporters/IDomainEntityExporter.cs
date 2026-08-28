using IFCConverter.Domain.Entities;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal interface IDomainEntityExporter
    {
        bool CanExport(Entity entity);
        void Export(Entity entity, IModel model, ExportContext context);
    }
}