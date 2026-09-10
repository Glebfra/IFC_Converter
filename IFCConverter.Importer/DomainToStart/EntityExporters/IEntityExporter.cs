using IFCConverter.Domain;
using IFCConverter.Domain.Entities;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal interface IEntityExporter
    {
        bool CanExport(Entity entity);
        void Export(Entity entity, EngineeringModel model, ExportContext context);
    }
}