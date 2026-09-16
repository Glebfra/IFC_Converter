using IFCConverter.Domain;
using IFCConverter.Importer.DomainToStart;
using IFCConverter.Importer.DomainToStart.Phases;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Importer.Pipeline
{
    internal sealed class DomainToStartPipeline
    {
        private readonly DomainToStartPhaseRegistry _registry = new DomainToStartPhaseRegistry();

        public void Execute(EngineeringModel model, IStartProject startProject)
        {
            ExportContext context = new ExportContext(startProject);
            foreach (IDomainToStartPhase phase in _registry.GetPhases())
            {
                phase.Execute(model, context);
            }
        }
    }
}