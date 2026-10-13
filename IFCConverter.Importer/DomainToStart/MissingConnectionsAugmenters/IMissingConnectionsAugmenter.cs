using IFCConverter.Domain;
using IFCConverter.Domain.Entities;

namespace IFCConverter.Importer.DomainToStart.MissingConnectionsAugmenters
{
    internal interface IMissingConnectionsAugmenter
    {
        bool CanAugment(Entity entity, EngineeringModel model, ExportContext context);
        void Augment(Entity entity, EngineeringModel model, ExportContext context);
    }
}