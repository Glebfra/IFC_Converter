using IFCConverter.Domain;
using IFCConverter.Domain.Entities;

namespace IFCConverter.Importer.DomainToStart.DomainAugmenters
{
    internal interface IDomainAugmenter
    {
        bool CanAugment(Entity entity);
        void Augment(Entity entity, EngineeringModel model, ExportContext context);
    }
}