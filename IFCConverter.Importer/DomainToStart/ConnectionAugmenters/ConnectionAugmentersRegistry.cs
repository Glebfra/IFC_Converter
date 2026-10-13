using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Importer.DomainToStart.ConnectionAugmenters
{
    internal sealed class ConnectionAugmentersRegistry : ReflectionRegistry<IConnectionAugmenter>, IConnectionAugmentersRegistry
    {
        public ConnectionAugmentersRegistry() : base(typeof(ConnectionAugmentersRegistry).Assembly)
        {
        }

        public IConnectionAugmenter Resolve(Connection connection)
        {
            return Resolve(augmenter => augmenter.CanAugment(connection));
        }

        public bool TryResolve(Connection connection, out IConnectionAugmenter augmenter)
        {
            return TryResolve(aug => aug.CanAugment(connection), out augmenter);
        }
    }
}