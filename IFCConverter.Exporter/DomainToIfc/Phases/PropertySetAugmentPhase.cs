using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.DomainToIfc.PropertySetAugmenters;
using Microsoft.Extensions.Logging;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.Phases
{
    [DomainToIfcPhase(1, typeof(EntityExportPhase))]
    public class PropertySetAugmentPhase : IDomainToIfcPhase
    {
        private readonly IPropertySetAugmentersRegistry _propertySetAugmentersRegistry = new PropertySetAugmenterRegistry();

        public void Execute(EngineeringModel domain, IModel model, ExportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<PropertySetAugmentPhase>();

            foreach (Entity domainEntity in domain.Entities)
            {
                foreach (IPropertySetAugmenter propertySetAugmenter in _propertySetAugmentersRegistry.ResolveAll(domainEntity, context))
                {
                    logger.LogInformation(
                        "Found property set augmenter {augmenter} for domain entity {entity} with id {id}",
                        propertySetAugmenter.GetType().Name,
                        domainEntity.GetType().Name,
                        domainEntity.Id
                    );
                    propertySetAugmenter.Augment(domainEntity, model, context);
                }
            }
        }
    }
}