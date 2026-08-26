using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.StartEntityImporters;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1)]
    public sealed class EntityImportPhase : IStartToDomainPhase
    {
        private readonly IStartEntityImporterRegistry _startEntityImporterRegistry = new StartEntityImporterRegistry();
        private readonly Logger _logger = Logger.GetInstance();
        
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