using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.StartToDomain
{
    internal sealed class EntityMetadataAugmenterRegistry : ReflectionRegistry<IEntityMetadataAugmenter>, IEntityMetadataAugmenterRegistry
    {
        public EntityMetadataAugmenterRegistry() : base(typeof(EntityMetadataAugmenterRegistry).Assembly)
        {
        }

        public IEntityMetadataAugmenter Resolve(IStartEntity entity, EngineeringModel model, StartMappingContext context)
        {
            return Resolve(augmenter => augmenter.CanResolve(entity, model, context));
        }

        public IEnumerable<IEntityMetadataAugmenter> ResolveAll(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            return ResolveAll(augmenter => augmenter.CanResolve(source, model, context));
        }

        public bool TryResolve(IStartEntity entity,  EngineeringModel model, StartMappingContext context, out IEntityMetadataAugmenter registration)
        {
            return TryResolve(augmenter => augmenter.CanResolve(entity, model, context), out registration);
        }
    }
}