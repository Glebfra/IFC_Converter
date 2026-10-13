using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.DomainToStart.MissingConnectionsAugmenters;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    [DomainToStartPhase(1, typeof(ConnectionAugmentPhase))]
    public sealed class MissingConnectionsAugmentPhase : IDomainToStartPhase
    {
        private readonly IMissingConnectionsAugmentersRegistry _registry = new MissingConnectionsAugmentersRegistry();
        
        public void Execute(EngineeringModel model, ExportContext context)
        {
            foreach (Entity entity in model.Entities.ToArray())
            {
                if (_registry.TryResolve(entity, model, context, out IMissingConnectionsAugmenter augmenter))
                    augmenter.Augment(entity, model, context);
            }
        }
    }
}