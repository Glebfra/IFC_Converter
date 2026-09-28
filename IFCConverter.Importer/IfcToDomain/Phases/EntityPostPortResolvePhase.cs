using IFCConverter.Domain;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.IfcToDomain.EntityPostPortResolvers;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    [IfcToDomainPhase(1, typeof(EntityPortResolvePhase))]
    public sealed class EntityPostPortResolvePhase : IIfcToDomainPhase
    {
        private readonly IEntityPostPortResolversRegistry _registry = new EntityPostPortResolversRegistry();
        
        public void Execute(IModel model, EngineeringModel domain, ImportContext context)
        {
            foreach (IIfcProduct ifcProduct in model.Instances.OfType<IIfcProduct>())
            {
                if (_registry.TryResolve(ifcProduct, out IEntityPostPortResolver resolver))
                    resolver.Resolve(ifcProduct, domain, context);
            }
        }
    }
}