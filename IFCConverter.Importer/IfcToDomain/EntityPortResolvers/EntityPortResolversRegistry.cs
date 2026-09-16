using IFCConverter.Domain;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers
{
    internal sealed class EntityPortResolversRegistry : ReflectionRegistry<IEntityPortResolver>, IEntityPortResolversRegistry
    {
        public EntityPortResolversRegistry() : base(typeof(EntityPortResolversRegistry).Assembly)
        {
        }

        public IEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return Resolve(resolver => resolver.CanResolve(product, model, context));
        }

        public bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out IEntityPortResolver resolver)
        {
            return TryResolve(res => res.CanResolve(product, model, context), out resolver);
        }
    }
}