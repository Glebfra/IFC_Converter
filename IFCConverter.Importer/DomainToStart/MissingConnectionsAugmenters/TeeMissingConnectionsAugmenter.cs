using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;

namespace IFCConverter.Importer.DomainToStart.MissingConnectionsAugmenters
{
    internal sealed class TeeMissingConnectionsAugmenter : IMissingConnectionsAugmenter
    {
        public bool CanAugment(Entity entity, EngineeringModel model, ExportContext context)
        {
            return entity is Tee && model.GetConnections(entity.Id).Count() != 3;
        }

        public void Augment(Entity entity, EngineeringModel model, ExportContext context)
        {
        }
    }
}