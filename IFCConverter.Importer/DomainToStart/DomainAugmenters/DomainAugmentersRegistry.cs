using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Importer.DomainToStart.DomainAugmenters
{
    internal sealed class DomainAugmentersRegistry : ReflectionRegistry<IDomainAugmenter>, IDomainAugmentersRegistry
    {
        public DomainAugmentersRegistry() : base(typeof(DomainAugmentersRegistry).Assembly)
        {
        }

        public IDomainAugmenter Resolve(Entity entity)
        {
            return Resolve(augmenter => augmenter.CanAugment(entity));
        }

        public bool TryResolve(Entity entity, out IDomainAugmenter augmenter)
        {
            return TryResolve(aug => aug.CanAugment(entity), out augmenter);
        }
    }
}