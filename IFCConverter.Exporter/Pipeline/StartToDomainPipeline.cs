using System;
using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.StartToDomain;
using Start.Interfaces;

namespace IFCConverter.Exporter.Pipeline
{
    internal sealed class StartToDomainPipeline
    {
        private readonly StartToDomainPhaseRegistry _phaseRegistry = new StartToDomainPhaseRegistry();

        public EngineeringModel Execute(IReadOnlyCollection<IStartEntity> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            EngineeringModel model = new EngineeringModel();
            StartMappingContext context = new StartMappingContext();
            
            foreach (IStartToDomainPhase startToDomainPhase in _phaseRegistry.GetPhases())
            {
                startToDomainPhase.Execute(source, model, context);
            }
            
            return model;
        }
    }
}