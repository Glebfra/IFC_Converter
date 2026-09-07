using System.Collections.Generic;
using IFCConverter.Exporter.StartToDomain.PortResolvers;
using IFCConverter.Utils.Reflection;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal sealed class PortResolverRegistry : ReflectionRegistry<IPortResolver>, IPortResolverRegistry
    {
        public PortResolverRegistry() : base(typeof(PortResolverRegistry).Assembly)
        {
        }

        public IPortResolver Resolve(IStartEntity source)
        {
            return Resolve(resolver => resolver.CanResolve(source));
        }

        public IEnumerable<IPortResolver> ResolveAll(IStartEntity source)
        {
            return ResolveAll(resolver => resolver.CanResolve(source));
        }

        public bool TryResolve(IStartEntity source, out IPortResolver resolver)
        {
            return TryResolve(res => res.CanResolve(source), out resolver);
        }
    }
}