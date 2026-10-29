using IFCConverter.Domain;
using IFCConverter.Importer.DomainToStart;
using IFCConverter.Importer.DomainToStart.Phases;
using IFCConverter.Start.Interfaces;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Importer.Pipeline
{
    internal sealed class DomainToStartPipeline
    {
        private readonly DomainToStartPhaseRegistry _registry = new DomainToStartPhaseRegistry();

        public void Execute(EngineeringModel model, IStartProject startProject, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<DomainToStartPipeline>();

            ExportContext context = new ExportContext(startProject);
            foreach (IDomainToStartPhase phase in _registry.GetPhases())
            {
                logger.LogInformation("Executing import phase: {phase}", phase.GetType().Name);
                phase.Execute(model, context, factory);
            }
        }
    }
}