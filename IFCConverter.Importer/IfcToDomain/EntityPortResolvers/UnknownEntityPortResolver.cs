using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain.EntityPortResolvers.UnknownEntityPortResolvers;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers
{
    internal sealed class UnknownEntityPortResolver : IEntityPortResolver
    {
        private readonly IUnknownEntityPortResolversRegistry _registry = new UnknownEntityPortResolversRegistry();

        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out _))
                return false;

            return context.ImportType == ImportType.UNKNOWN;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, model, context, out IUnknownEntityPortResolver resolver))
                resolver.Resolve(product, model, context);
        }
    }
}