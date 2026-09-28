using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Importer.IfcToDomain.EntityPortResolvers.ElbowEntityPortResolvers;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers
{
    internal sealed class ElbowEntityPortResolver : IEntityPortResolver
    {
        private readonly IElbowEntityPortResolversRegistry _registry = new ElbowEntityPortResolversRegistry();
        
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out EntityId id))
                return false;

            Entity entity = model.GetEntity(id);
            return entity is Elbow;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, model, context, out IElbowEntityPortResolver resolver))
                resolver.Resolve(product, model, context);
        }
    }
}