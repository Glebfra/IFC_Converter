using IFCConverter.Domain;
using IFCConverter.Domain.Entities;

namespace IFCConverter.Importer.DomainToStart.MissingConnectionsAugmenters
{
    internal interface IMissingConnectionsAugmentersRegistry
    {
        IMissingConnectionsAugmenter Resolve(Entity entity, EngineeringModel model, ExportContext context);
        bool TryResolve(Entity entity, EngineeringModel model, ExportContext context, out IMissingConnectionsAugmenter augmenter);
    }
}