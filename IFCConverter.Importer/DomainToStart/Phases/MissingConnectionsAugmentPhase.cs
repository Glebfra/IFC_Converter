using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.DomainToStart.MissingConnectionsAugmenters;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    [DomainToStartPhase(1, typeof(ConnectionAugmentPhase))]
    public sealed class MissingConnectionsAugmentPhase : IDomainToStartPhase
    {
        private readonly IMissingConnectionsAugmentersRegistry _registry = new MissingConnectionsAugmentersRegistry();
        
        public void Execute(EngineeringModel model, ExportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<MissingConnectionsAugmentPhase>();
            
            foreach (Entity entity in model.Entities.ToArray())
            {
                if (_registry.TryResolve(entity, model, context, out IMissingConnectionsAugmenter augmenter))
                {
                    logger.LogInformation(
                        "Found missing connection augmenter {augmenter} for domain entity {type} with id {id}",
                        augmenter.GetType().Name,
                        entity.GetType().Name,
                        entity.Id.Value
                    );
                    augmenter.Augment(entity, model, context);
                }
            }
        }
    }
}