using IFCConverter.Exporter.StartToDomain.ConnectionResolvers;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.StartToDomain
{
    internal sealed class ConnectionResolverRegistry : ReflectionRegistry<IConnectionResolver>, IConnectionResolverRegistry
    {
        public ConnectionResolverRegistry() : base(typeof(ConnectionResolverRegistry).Assembly)
        {
        }

        public IConnectionResolver Resolve(IStartEntity source)
        {
            return Resolve(resolver => resolver.CanResolve(source));
        }

        public bool TryResolve(IStartEntity source, out IConnectionResolver result)
        {
            return TryResolve(resolver => resolver.CanResolve(source), out result);
        }
    }
}