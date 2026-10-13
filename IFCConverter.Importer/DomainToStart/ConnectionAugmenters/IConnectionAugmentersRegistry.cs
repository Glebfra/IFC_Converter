using IFCConverter.Domain.Topology;

namespace IFCConverter.Importer.DomainToStart.ConnectionAugmenters
{
    internal interface IConnectionAugmentersRegistry
    {
        IConnectionAugmenter Resolve(Connection connection);
        bool TryResolve(Connection connection, out IConnectionAugmenter augmenter);
    }
}