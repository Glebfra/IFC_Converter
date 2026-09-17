using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.DomainToStart.EntityExporters;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    [DomainToStartPhase(1, typeof(SegmentsPortAugmentPhase))]
    public sealed class EntityExportPhase : IDomainToStartPhase
    {
        private readonly IEntityExportersRegistry _registry = new EntityExportersRegistry();

        public void Execute(EngineeringModel model, ExportContext context)
        {
            foreach (Entity entity in model.Entities)
            {
                if (_registry.TryResolve(entity, out IEntityExporter exporter))
                    exporter.Export(entity, model, context);
            }
        }
    }
}