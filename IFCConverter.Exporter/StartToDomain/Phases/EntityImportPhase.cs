using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.StartEntityImporters;
using IFCConverter.Start.Interfaces;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1)]
    public sealed class EntityImportPhase : IStartToDomainPhase
    {
        private readonly IStartEntityImporterRegistry _startEntityImporterRegistry = new StartEntityImporterRegistry();

        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<EntityImportPhase>();

            foreach (IStartEntity startEntity in source)
            {
                if (_startEntityImporterRegistry.TryResolve(startEntity, out IStartEntityImporter importer))
                {
                    logger.LogInformation(
                        "Found importer {importer} for start entity {entity} with name {name}",
                        importer.GetType().Name,
                        startEntity.GetType().Name, startEntity.Name
                    );
                    importer.Import(startEntity, model, context);
                }
            }
        }
    }
}