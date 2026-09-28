using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Importer.IfcToDomain.EntityPortResolvers.TeeEntityPortResolvers;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers
{
    internal sealed class TeeEntityPortResolver : IEntityPortResolver
    {
        private readonly ITeeEntityPortResolversRegistry _registry = new TeeEntityPortResolversRegistry();
        
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out EntityId id))
                return false;

            Entity entity = model.GetEntity(id);
            return entity is Tee;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, model, context, out ITeeEntityPortResolver resolver))
                resolver.Resolve(product, model, context);
        }
    }
}