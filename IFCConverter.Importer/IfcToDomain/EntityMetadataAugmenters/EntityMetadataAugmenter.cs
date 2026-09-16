using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityMetadataAugmenters
{
    internal sealed class EntityMetadataAugmenter : IEntityMetadataAugmenter
    {
        public bool CanAugment(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return context.TryGetEntityId(product, out _);
        }

        public void Augment(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Entity entity = model.GetEntity(context.GetEntityId(product));
            entity.Metadata.Name = product.Name;
        }
    }
}