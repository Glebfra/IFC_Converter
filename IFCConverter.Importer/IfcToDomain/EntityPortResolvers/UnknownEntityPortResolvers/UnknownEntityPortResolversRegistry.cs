using System;
using IFCConverter.Domain;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.UnknownEntityPortResolvers
{
    internal sealed class UnknownEntityPortResolversRegistry : ReflectionRegistry<IUnknownEntityPortResolver>, IUnknownEntityPortResolversRegistry
    {
        public UnknownEntityPortResolversRegistry() : base(typeof(UnknownEntityPortResolversRegistry).Assembly)
        {
        }

        public IUnknownEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return Resolve(resolver => resolver.CanResolve(product, model, context));
            throw new NotImplementedException();
        }

        public bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out IUnknownEntityPortResolver resolver)
        {
            return TryResolve(res => res.CanResolve(product, model, context), out resolver);
        }
    }
}