using System.Collections.Generic;
using System.Reflection;
using IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.StartToDomain
{
    internal sealed class EntityMetadataAugmenterRegistry : ReflectionRegistry<IEntityMetadataAugmenter>, IEntityMetadataAugmenterRegistry
    {
        public EntityMetadataAugmenterRegistry() : base(typeof(EntityMetadataAugmenterRegistry).Assembly)
        {
        }

        public IEnumerable<IEntityMetadataAugmenter> ResolveAll(IStartEntity source, StartMappingContext context)
        {
            return ResolveAll(augmenter => augmenter.CanResolve(source, context));
        }
    }
}