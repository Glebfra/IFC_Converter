using IFCConverter.Domain.Entities;
using IFCConverter.Start.API;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.SegmentDomainExporters
{
    internal interface ISegmentDomainEntityExporter
    {
        bool CanExport(StartElementTypeEnum type);
        void Export(Entity entity, IModel model, ExportContext context);
    }
}