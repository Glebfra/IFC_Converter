using IFCConverter.Domain.Entities;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.PropertySetAugmenters
{
    internal interface IPropertySetAugmenter
    {
        bool CanAugment(Entity entity, ExportContext context);
        void Augment(Entity entity, IModel model, ExportContext context);
    }
}