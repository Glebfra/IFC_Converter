using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.StartEntityImporters;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1)]
    public sealed class EntityImportPhase : IStartToDomainPhase
    {
        private readonly IStartEntityImporterRegistry _startEntityImporterRegistry = new StartEntityImporterRegistry();
        
        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context)
        {
            foreach (IStartEntity startEntity in source)
            {
                if (_startEntityImporterRegistry.TryResolve(startEntity, out IStartEntityImporter importer))
                    importer.Import(startEntity, model, context);
            }
        }
    }
}