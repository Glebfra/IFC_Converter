using System.Collections.Generic;
using IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters;
using IFCConverter.Utils.Reflection;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal sealed class EntityMetadataAugmenterRegistry : ReflectionRegistry<IEntityMetadataAugmenter>, IEntityMetadataAugmenterRegistry
    {
        public EntityMetadataAugmenterRegistry() : base(typeof(EntityMetadataAugmenterRegistry).Assembly)
        {
        }

        public IEntityMetadataAugmenter Resolve(IStartEntity entity, StartMappingContext context)
        {
            return Resolve(augmenter => augmenter.CanResolve(entity, context));
        }

        public IEnumerable<IEntityMetadataAugmenter> ResolveAll(IStartEntity source, StartMappingContext context)
        {
            return ResolveAll(augmenter => augmenter.CanResolve(source, context));
        }

        public bool TryResolve(IStartEntity entity, StartMappingContext context, out IEntityMetadataAugmenter registration)
        {
            return TryResolve(augmenter => augmenter.CanResolve(entity, context), out registration);
        }
    }
}