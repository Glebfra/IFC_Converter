using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class EntityMetadataMaterialAugmenter : IEntityMetadataAugmenter
    {
        private Id<EntityId> _id;
        private IStartMaterializedEntity _startMaterializedEntity;

        public bool CanResolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out _id))
                return false;

            if (!(source is IStartMaterializedEntity startMaterializedEntity))
                return false;
            _startMaterializedEntity = startMaterializedEntity;

            return true;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Entity entity = model.GetEntity(_id);
            entity.Metadata.MaterialName = _startMaterializedEntity.MaterialName;
        }
    }
}