using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class EntityMetadataMaterialAugmenter : IEntityMetadataAugmenter
    {
        public bool CanResolve(IStartEntity source, StartMappingContext context)
        {
            return context.TryGetEntityId(source, out _) && source is IStartMaterializedEntity;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Entity entity = model.GetEntity(context.GetEntityId(source));
            IStartMaterializedEntity materializedEntity = (IStartMaterializedEntity)source;
            entity.Metadata.MaterialName = materializedEntity.MaterialName;
        }
    }
}