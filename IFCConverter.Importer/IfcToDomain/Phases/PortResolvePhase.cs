using IFCConverter.Domain;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.IfcToDomain.EntityPortResolvers;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    [IfcToDomainPhase(1, typeof(EntityMetadataAugmentPhase))]
    public sealed class PortResolvePhase : IIfcToDomainPhase
    {
        private readonly IEntityPortResolversRegistry _registry = new EntityPortResolversRegistry();
        
        public void Execute(IModel model, EngineeringModel domain, ImportContext context)
        {
            foreach (IIfcProduct product in model.Instances.OfType<IIfcProduct>())
            {
                if (_registry.TryResolve(product, domain, context, out IEntityPortResolver resolver))
                    resolver.Resolve(product, domain, context);
            }
        }
    }
}