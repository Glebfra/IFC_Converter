using IFCConverter.Domain;
using IFCConverter.Domain.Identity;
using IFCConverter.Importer.IfcToDomain.EntityPortResolvers.AvevaEntityPortResolvers;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers
{
    internal sealed class AvevaEntityPortResolver : IEntityPortResolver
    {
        private readonly IAvevaEntityPortResolversRegistry _registry = new AvevaEntityPortResolversRegistry();

        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out EntityId id))
                return false;

            return context.ImportType == ImportType.AVEVA;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, model, context, out IAvevaEntityPortResolver resolver))
                resolver.Resolve(product, model, context);
        }
    }
}