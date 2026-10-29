using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.DomainToIfc.MaterialAugmenters;
using Microsoft.Extensions.Logging;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.Phases
{
    [DomainToIfcPhase(1, typeof(PropertySetAugmentPhase))]
    internal sealed class MaterialAugmentPhase : IDomainToIfcPhase
    {
        private readonly IMaterialAugmentersRegistry _registry = new MaterialAugmentersRegistry();

        public void Execute(EngineeringModel domain, IModel model, ExportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<MaterialAugmentPhase>();

            foreach (Entity entity in domain.Entities)
            {
                if (_registry.TryResolve(entity, out IMaterialAugmenter augmenter))
                {
                    logger.LogInformation(
                        "Found material augmenter {augmenter} for domain entity {entity} with id {id}",
                        augmenter.GetType().Name,
                        entity.GetType().Name,
                        entity.Id
                    );
                    augmenter.Augment(entity, model, context);
                }
            }
        }
    }
}