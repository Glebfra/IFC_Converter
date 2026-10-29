using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters;
using IFCConverter.Start.Interfaces;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(EntityImportPhase))]
    public class EntityMetadataAugmentPhase : IStartToDomainPhase
    {
        private readonly IEntityMetadataAugmenterRegistry _augmenterRegistry = new EntityMetadataAugmenterRegistry();

        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<EntityMetadataAugmentPhase>();

            foreach (IStartEntity startEntity in source)
            {
                foreach (IEntityMetadataAugmenter entityMetadataAugmenter in _augmenterRegistry.ResolveAll(startEntity, model, context))
                {
                    logger.LogInformation(
                        "Found metadata augmenter {augmenter} for start entity {entity} with name {name}",
                        entityMetadataAugmenter.GetType().Name,
                        startEntity.GetType().Name,
                        startEntity.Name
                    );
                    entityMetadataAugmenter.Augment(startEntity, model, context);
                }
            }
        }
    }
}