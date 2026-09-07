using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.DomainToIfc.PropertySetAugmenters;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.Phases
{
    [DomainToIfcPhase(1, typeof(EntityExportPhase))]
    public class PropertySetAugmentPhase : IDomainToIfcPhase
    {
        private readonly IPropertySetAugmentersRegistry _propertySetAugmentersRegistry = new PropertySetAugmenterRegistry();

        public void Execute(EngineeringModel domain, IModel model, ExportContext context)
        {
            foreach (Entity domainEntity in domain.Entities)
            {
                foreach (IPropertySetAugmenter propertySetAugmenter in _propertySetAugmentersRegistry.ResolveAll(domainEntity, context))
                {
                    propertySetAugmenter.Augment(domainEntity, model, context);
                }
            }
        }
    }
}