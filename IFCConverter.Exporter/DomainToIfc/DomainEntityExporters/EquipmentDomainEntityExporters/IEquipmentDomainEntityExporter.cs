using IFCConverter.Domain.Entities;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.EquipmentDomainEntityExporters
{
    internal interface IEquipmentDomainEntityExporter
    {
        bool CanExport(Equipment equipment);
        void Export(Equipment equipment, IModel model, ExportContext context);
    }
}