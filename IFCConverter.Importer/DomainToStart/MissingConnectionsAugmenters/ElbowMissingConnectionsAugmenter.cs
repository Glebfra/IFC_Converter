using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;

namespace IFCConverter.Importer.DomainToStart.MissingConnectionsAugmenters
{
    internal sealed class ElbowMissingConnectionsAugmenter : IMissingConnectionsAugmenter
    {
        public bool CanAugment(Entity entity, EngineeringModel model, ExportContext context)
        {
            return entity is Elbow && model.GetConnections(entity.Id).Count() != 2;
        }

        public void Augment(Entity entity, EngineeringModel model, ExportContext context)
        {
        }
    }
}