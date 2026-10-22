using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class EntityMetadataPropertiesAugmenter : IEntityMetadataAugmenter
    {
        private Id<EntityId> _id;
        
        public bool CanResolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out _id))
                return false;

            return true;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Entity entity = model.GetEntity(_id);
            foreach (KeyValuePair<string, string> kvp in source.GetData())
            {
                entity.Metadata.Properties.Add(kvp.Key, kvp.Value);
            }
        }
    }
}