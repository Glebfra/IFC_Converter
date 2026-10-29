using System;
using IFCConverter.Domain;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.IfcToDomain.EntityMetadataAugmenters;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    [IfcToDomainPhase(1, typeof(EntityImportPhase))]
    public sealed class EntityMetadataAugmentPhase : IIfcToDomainPhase
    {
        private readonly IEntityMetadataAugmentersRegistry _registry = new EntityMetadataAugmentersRegistry();

        public void Execute(IModel model, EngineeringModel domain, ImportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<EntityMetadataAugmentPhase>();
            
            foreach (IIfcProduct product in model.Instances.OfType<IIfcProduct>())
            {
                foreach (IEntityMetadataAugmenter augmenter in _registry.ResolveAll(product, domain, context))
                {
                    try
                    {
                        logger.LogInformation("Found entity metadata augmenter {augmenter} for product with id {id}", augmenter.GetType().Name, product.GlobalId);
                        augmenter.Augment(product, domain, context);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e.Message);
                    }
                }
            }
        }
    }
}