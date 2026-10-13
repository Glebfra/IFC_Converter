using IFCConverter.Domain;
using IFCConverter.Domain.Topology;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.DomainToStart.ConnectionAugmenters;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    [DomainToStartPhase(1)]
    public sealed class ConnectionAugmentPhase : IDomainToStartPhase
    {
        private readonly IConnectionAugmentersRegistry _registry = new ConnectionAugmentersRegistry();
        
        public void Execute(EngineeringModel model, ExportContext context)
        {
            foreach (Connection connection in model.Connections)
            {
                if (_registry.TryResolve(connection, out IConnectionAugmenter augmenter))
                    augmenter.Augment(connection, model, context);
            }
        }
    }
}