using IFCConverter.Domain;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.TeeEntityPortResolvers
{
    internal sealed class TeeEntityPortResolversRegistry : ReflectionRegistry<ITeeEntityPortResolver>, ITeeEntityPortResolversRegistry
    {
        public TeeEntityPortResolversRegistry() : base(typeof(TeeEntityPortResolversRegistry).Assembly)
        {
        }

        public ITeeEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return Resolve(resolver => resolver.CanResolve(product, model, context));
        }

        public bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out ITeeEntityPortResolver resolver)
        {
            return TryResolve(res => res.CanResolve(product, model, context), out resolver);
        }
    }
}