using IFCConverter.Domain;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.ElbowEntityPortResolvers
{
    internal sealed class ElbowEntityPortResolversRegistry : ReflectionRegistry<IElbowEntityPortResolver>, IElbowEntityPortResolversRegistry
    {
        public ElbowEntityPortResolversRegistry() : base(typeof(ElbowEntityPortResolversRegistry).Assembly)
        {
        }

        public IElbowEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return Resolve(resolver => resolver.CanResolve(product, model, context));
        }

        public bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out IElbowEntityPortResolver resolver)
        {
            return TryResolve(res => res.CanResolve(product, model, context), out resolver);
        }
    }
}