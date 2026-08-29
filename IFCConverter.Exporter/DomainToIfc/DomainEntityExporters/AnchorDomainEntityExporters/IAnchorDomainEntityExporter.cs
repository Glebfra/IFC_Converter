using IFCConverter.Domain.Entities;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal interface IAnchorDomainEntityExporter
    {
        bool CanExport(Anchor anchor);
        void Export(Anchor anchor, IModel model, ExportContext context);
    }
}