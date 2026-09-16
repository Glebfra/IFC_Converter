using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityMetadataAugmenters
{
    internal sealed class EntityMetadataAugmentersRegistry : ReflectionRegistry<IEntityMetadataAugmenter>, IEntityMetadataAugmentersRegistry
    {
        public EntityMetadataAugmentersRegistry() : base(typeof(EntityMetadataAugmentersRegistry).Assembly)
        {
        }

        public IEnumerable<IEntityMetadataAugmenter> ResolveAll(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return ResolveAll(augmenter => augmenter.CanAugment(product, model, context));
        }
    }
}