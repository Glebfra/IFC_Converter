using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(EntityImportPhase))]
    public class EntityMetadataAugmentPhase : IStartToDomainPhase
    {
        private readonly IEntityMetadataAugmenterRegistry _augmenterRegistry = new EntityMetadataAugmenterRegistry();
        
        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context)
        {
            foreach (IStartEntity startEntity in source)
            {
                foreach (IEntityMetadataAugmenter entityMetadataAugmenter in _augmenterRegistry.ResolveAll(startEntity, context))
                {
                    entityMetadataAugmenter.Augment(startEntity, model, context);
                }
            }
        }
    }
}