using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.StartEntityImporters;
using IFCConverter.Utils.Diagnostics;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1)]
    public sealed class EntityImportPhase : IStartToDomainPhase
    {
        private readonly Logger _logger = Logger.GetInstance();
        private readonly IStartEntityImporterRegistry _startEntityImporterRegistry = new StartEntityImporterRegistry();

        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context)
        {
            _logger.Info($"Starting '{nameof(EntityImportPhase)}'.");

            foreach (IStartEntity startEntity in source)
            {
                if (_startEntityImporterRegistry.TryResolve(startEntity, out IStartEntityImporter importer))
                    importer.Import(startEntity, model, context);
            }

            _logger.Info($"Finished '{nameof(EntityImportPhase)}'.");
        }
    }
}