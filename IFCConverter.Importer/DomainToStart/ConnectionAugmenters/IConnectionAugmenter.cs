using IFCConverter.Domain;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Importer.DomainToStart.ConnectionAugmenters
{
    internal interface IConnectionAugmenter
    {
        bool CanAugment(Connection connection);
        void Augment(Connection connection, EngineeringModel model, ExportContext context);
    }
}