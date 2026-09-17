using System.Collections.Generic;
using IFCConverter.Domain.Entities;

namespace IFCConverter.Exporter.DomainToIfc.PropertySetAugmenters
{
    internal interface IPropertySetAugmentersRegistry
    {
        IPropertySetAugmenter Resolve(Entity entity, ExportContext context);
        IEnumerable<IPropertySetAugmenter> ResolveAll(Entity entity, ExportContext context);
        bool TryResolve(Entity entity, ExportContext context, out IPropertySetAugmenter exporter);
    }
}