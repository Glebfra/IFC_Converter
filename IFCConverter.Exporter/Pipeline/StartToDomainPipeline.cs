using System;
using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.StartToDomain;
using IFCConverter.Start.Interfaces;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Exporter.Pipeline
{
    internal sealed class StartToDomainPipeline
    {
        private readonly StartToDomainPhaseRegistry _phaseRegistry = new StartToDomainPhaseRegistry();

        public EngineeringModel Execute(IReadOnlyCollection<IStartEntity> source, ILoggerFactory factory)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            ILogger logger = factory.CreateLogger<StartToDomainPipeline>();

            EngineeringModel model = new EngineeringModel();
            StartMappingContext context = new StartMappingContext();

            foreach (IStartToDomainPhase startToDomainPhase in _phaseRegistry.GetPhases())
            {
                logger.LogInformation("Executing phase {phase}", startToDomainPhase.GetType().Name);
                startToDomainPhase.Execute(source, model, context, factory);
            }

            return model;
        }
    }
}