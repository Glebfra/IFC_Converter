using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.IfcToDomain.SegmentSplitters;
using Microsoft.Extensions.Logging;
using Xbim.Common;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    [IfcToDomainPhase(1, typeof(EntityMetadataAugmentPhase))]
    public sealed class SegmentSplitPhase : IIfcToDomainPhase
    {
        private readonly ISegmentSplittersRegistry _registry = new SegmentSplittersRegistry();
        
        public void Execute(IModel model, EngineeringModel domain, ImportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<SegmentSplitPhase>();
            
            foreach (Entity entity in domain.Entities.ToArray())
            {
                try
                {
                    if (_registry.TryResolve(entity, domain, context, out ISegmentSplitter splitter))
                    {
                        logger.LogInformation("Found splitter {splitter} for domain entity {type} with id {id}", splitter.GetType().Name,
                            entity.GetType().Name, entity.Id.Value);
                        splitter.Split(entity, domain, context);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                }
            }
        }
    }
}