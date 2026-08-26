using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(EntityImportPhase))]
    public class EntityMetadataAugmentPhase : IStartToDomainPhase
    {
        private readonly IEntityMetadataAugmenterRegistry _augmenterRegistry = new EntityMetadataAugmenterRegistry();
        private readonly Logger _logger = Logger.GetInstance();
        
        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context)
        {
            _logger.Info($"Starting '{nameof(EntityMetadataAugmentPhase)}'.");
            
            foreach (IStartEntity startEntity in source)
            {
                foreach (IEntityMetadataAugmenter entityMetadataAugmenter in _augmenterRegistry.ResolveAll(startEntity, context))
                {
                    entityMetadataAugmenter.Augment(startEntity, model, context);
                }
            }
            
            _logger.Info($"Finished '{nameof(EntityMetadataAugmentPhase)}'.");
        }
    }
}