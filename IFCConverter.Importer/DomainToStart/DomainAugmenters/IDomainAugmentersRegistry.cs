using IFCConverter.Domain.Entities;

namespace IFCConverter.Importer.DomainToStart.DomainAugmenters
{
    internal interface IDomainAugmentersRegistry
    {
        IDomainAugmenter Resolve(Entity entity);
        bool TryResolve(Entity entity, out IDomainAugmenter augmenter);
    }
}