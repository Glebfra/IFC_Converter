using IFCConverter.Domain;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.IfcToDomain.EntityMetadataAugmenters;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    [IfcToDomainPhase(1, typeof(EntityImportPhase))]
    public sealed class EntityMetadataAugmentPhase : IIfcToDomainPhase
    {
        private readonly IEntityMetadataAugmentersRegistry _registry = new EntityMetadataAugmentersRegistry();
        
        public void Execute(IModel model, EngineeringModel domain, ImportContext context)
        {
            foreach (IIfcProduct product in model.Instances.OfType<IIfcProduct>())
            {
                foreach (IEntityMetadataAugmenter augmenter in _registry.ResolveAll(product, domain, context))
                {
                    augmenter.Augment(product, domain, context);
                }
            }
        }
    }
}