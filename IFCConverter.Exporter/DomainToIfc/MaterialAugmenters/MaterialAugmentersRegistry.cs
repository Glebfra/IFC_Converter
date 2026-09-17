using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.DomainToIfc.MaterialAugmenters
{
    internal sealed class MaterialAugmentersRegistry : ReflectionRegistry<IMaterialAugmenter>, IMaterialAugmentersRegistry
    {
        public MaterialAugmentersRegistry() : base(typeof(MaterialAugmentersRegistry).Assembly)
        {
        }

        public IMaterialAugmenter Resolve(Entity entity)
        {
            return Resolve(augmenter => augmenter.CanAugment(entity));
        }

        public bool TryResolve(Entity entity, out IMaterialAugmenter augmenter)
        {
            return TryResolve(aug => aug.CanAugment(entity), out augmenter);
        }
    }
}