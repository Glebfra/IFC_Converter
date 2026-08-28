using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class EntityMetadataPropertiesAugmenter : IEntityMetadataAugmenter
    {
        public bool CanResolve(IStartEntity source, StartMappingContext context)
        {
            return context.TryGetEntityId(source, out _);
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Entity entity = model.GetEntity(context.GetEntityId(source));
            foreach (KeyValuePair<string, string> kvp in source.GetData())
            {
                entity.Metadata.Properties.Add(kvp.Key, kvp.Value);
            }
        }
    }
}