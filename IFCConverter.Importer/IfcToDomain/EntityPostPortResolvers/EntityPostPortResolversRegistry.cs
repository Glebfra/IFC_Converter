using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPostPortResolvers
{
    internal sealed class EntityPostPortResolversRegistry : ReflectionRegistry<IEntityPostPortResolver>, IEntityPostPortResolversRegistry
    {
        public EntityPostPortResolversRegistry() : base(typeof(EntityPostPortResolversRegistry).Assembly)
        {
        }

        public IEntityPostPortResolver Resolve(IIfcProduct product)
        {
            return Resolve(resolver => resolver.CanResolve(product));
        }

        public bool TryResolve(IIfcProduct product, out IEntityPostPortResolver resolver)
        {
            return TryResolve(res => res.CanResolve(product), out resolver);
        }
    }
}