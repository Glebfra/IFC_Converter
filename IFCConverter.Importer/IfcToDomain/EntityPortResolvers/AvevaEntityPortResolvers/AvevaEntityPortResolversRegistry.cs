using IFCConverter.Domain;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.AvevaEntityPortResolvers
{
    internal sealed class AvevaEntityPortResolversRegistry : ReflectionRegistry<IAvevaEntityPortResolver>, IAvevaEntityPortResolversRegistry
    {
        public AvevaEntityPortResolversRegistry() : base(typeof(AvevaEntityPortResolversRegistry).Assembly)
        {
        }

        public IAvevaEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return Resolve(resolver => resolver.CanResolve(product, model, context));
        }

        public bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out IAvevaEntityPortResolver resolver)
        {
            return TryResolve(res => res.CanResolve(product, model, context), out resolver);
        }
    }
}