using IFCConverter.Domain.Entities;
using IFCConverter.Start.API;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal interface IAnchorDomainEntityExporter
    {
        bool CanExport(StartElementTypeEnum type);
        void Export(Anchor anchor, IModel model, ExportContext context);
    }
}