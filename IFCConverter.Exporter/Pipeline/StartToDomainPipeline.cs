using System;
using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.StartToDomain;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.Pipeline
{
    internal sealed class StartToDomainPipeline
    {
        private readonly StartToDomainPhaseRegistry _phaseRegistry = new StartToDomainPhaseRegistry();
        private readonly Logger _logger = Logger.GetInstance();

        public EngineeringModel Execute(IReadOnlyCollection<IStartEntity> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            _logger.Info($"Starting '{nameof(StartToDomainPipeline)}'.");
            
            EngineeringModel model = new EngineeringModel();
            StartMappingContext context = new StartMappingContext();
            
            foreach (IStartToDomainPhase startToDomainPhase in _phaseRegistry.GetPhases())
            {
                startToDomainPhase.Execute(source, model, context);
            }
            
            _logger.Info($"Finished '{nameof(StartToDomainPipeline)}'.");
            
            return model;
        }
    }
}