using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.IfcToDomain.EntityImporters;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    [IfcToDomainPhase(1)]
    public sealed class EntityImportPhase : IIfcToDomainPhase
    {
        private readonly IIfcEntityImportersRegistry _registry = new IfcEntityImportersRegistry();
        private readonly IIfcPostEntityImportersRegistry _postRegistry = new IfcPostEntityImportersRegistry();
        
        public void Execute(IModel model, EngineeringModel domain, ImportContext context)
        {
            List<IIfcProduct> notImportedProducts = new List<IIfcProduct>();
            
            foreach (IIfcProduct product in model.Instances.OfType<IIfcProduct>())
            {
                if (_registry.TryResolve(product, context, out IIfcEntityImporter importer))
                    importer.Import(product, domain, context);
                else 
                    notImportedProducts.Add(product);
            }

            foreach (IIfcProduct product in notImportedProducts)
            {
                if (_postRegistry.TryResolve(product, context, out IIfcPostEntityImporter postImporter))
                    postImporter.Import(product, domain, context);
            }
        }
    }
}