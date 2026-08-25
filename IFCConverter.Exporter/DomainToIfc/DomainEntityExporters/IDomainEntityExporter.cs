using IFCConverter.Domain.Entities;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal interface IDomainEntityExporter
    {
        bool CanExport(Entity entity);
        IIfcProduct Export(Entity entity, IModel model, ExportContext context);
    }
}