using IFCConverter.Domain.Entities;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.MaterialAugmenters
{
    internal interface IMaterialAugmenter
    {
        bool CanAugment(Entity entity);
        void Augment(Entity entity, IModel model, ExportContext context);
    }
}