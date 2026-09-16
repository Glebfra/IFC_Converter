using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.DomainToStart.DomainAugmenters;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    [DomainToStartPhase(1)]
    internal sealed class DomainAugmentPhase : IDomainToStartPhase
    {
        private readonly IDomainAugmentersRegistry _registry = new DomainAugmentersRegistry();

        public void Execute(EngineeringModel model, ExportContext context)
        {
            foreach (Entity entity in model.Entities.ToArray())
            {
                if (_registry.TryResolve(entity, out IDomainAugmenter augmenter))
                    augmenter.Augment(entity, model, context);
            }
        }
    }
}