using IFCConverter.Domain.Entities;

namespace IFCConverter.Exporter.DomainToIfc.MaterialAugmenters
{
    internal interface IMaterialAugmentersRegistry
    {
        IMaterialAugmenter Resolve(Entity entity);
        bool TryResolve(Entity entity, out IMaterialAugmenter augmenter);
    }
}