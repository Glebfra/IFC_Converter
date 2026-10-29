using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.DomainToStart.EntityExporters;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    [DomainToStartPhase(1, typeof(MissingConnectionsAugmentPhase))]
    public sealed class EntityExportPhase : IDomainToStartPhase
    {
        private readonly IEntityExportersRegistry _registry = new EntityExportersRegistry();

        public void Execute(EngineeringModel model, ExportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<EntityExportPhase>();
            
            foreach (Entity entity in model.Entities)
            {
                if (_registry.TryResolve(entity, out IEntityExporter exporter))
                {
                    logger.LogInformation(
                        "Found exporter {exporter} for domain entity {type} with {id}",
                        exporter.GetType().Name,
                        entity.GetType().Name,
                        entity.Id.Value
                    );
                    exporter.Export(entity, model, context);
                }
            }
            
            logger.LogInformation("Converted {count} to start entities", context.StartEntities.Count());
        }
    }
}