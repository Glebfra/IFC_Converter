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
        
        public void Execute(IModel model, EngineeringModel domain, ImportContext context)
        {
            foreach (IIfcProduct product in model.Instances.OfType<IIfcProduct>())
            {
                if (_registry.TryResolve(product, context, out IIfcEntityImporter importer))
                    importer.Import(product, domain, context);
            }
        }
    }
}