using System;
using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.IfcToDomain.EntityImporters;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    [IfcToDomainPhase(1)]
    public sealed class EntityImportPhase : IIfcToDomainPhase
    {
        private readonly IIfcPostEntityImportersRegistry _postRegistry = new IfcPostEntityImportersRegistry();
        private readonly IIfcEntityImportersRegistry _registry = new IfcEntityImportersRegistry();

        public void Execute(IModel model, EngineeringModel domain, ImportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<EntityImportPhase>();

            List<IIfcProduct> notImportedProducts = new List<IIfcProduct>();

            foreach (IIfcProduct product in model.Instances.OfType<IIfcProduct>())
            {
                try
                {
                    if (_registry.TryResolve(product, context, out IIfcEntityImporter importer))
                    {
                        logger.LogInformation("Found importer {importer} for product with id {id}", importer.GetType().Name, product.GlobalId);
                        importer.Import(product, domain, context);
                    }
                    else
                    {
                        logger.LogInformation("Cannot find importer for product with id {id}. Adding to IIfcPostEntityImporters", product.GlobalId);
                        notImportedProducts.Add(product);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                }
            }

            foreach (IIfcProduct product in notImportedProducts)
            {
                try
                {
                    if (_postRegistry.TryResolve(product, context, out IIfcPostEntityImporter postImporter))
                    {
                        logger.LogInformation("Found post importer {importer} for product with id {id}", postImporter.GetType().Name, product.GlobalId);
                        postImporter.Import(product, domain, context);
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