using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Importer.DomainToStart.MissingConnectionsAugmenters
{
    internal sealed class MissingConnectionsAugmentersRegistry : ReflectionRegistry<IMissingConnectionsAugmenter>, IMissingConnectionsAugmentersRegistry
    {
        public MissingConnectionsAugmentersRegistry() : base(typeof(MissingConnectionsAugmentersRegistry).Assembly)
        {
        }

        public IMissingConnectionsAugmenter Resolve(Entity entity, EngineeringModel model, ExportContext context)
        {
            return Resolve(augmenter => augmenter.CanAugment(entity, model, context));
        }

        public bool TryResolve(Entity entity, EngineeringModel model, ExportContext context, out IMissingConnectionsAugmenter augmenter)
        {
            return TryResolve(aug => aug.CanAugment(entity, model, context), out augmenter);
        }
    }
}