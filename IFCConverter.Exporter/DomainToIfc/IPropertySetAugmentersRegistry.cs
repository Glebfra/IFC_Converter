using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.DomainToIfc.PropertySetAugmenters;

namespace IFCConverter.Exporter.DomainToIfc
{
    internal interface IPropertySetAugmentersRegistry
    {
        IPropertySetAugmenter Resolve(Entity entity, ExportContext context);
        IEnumerable<IPropertySetAugmenter> ResolveAll(Entity entity, ExportContext context);
        bool TryResolve(Entity entity, ExportContext context, out IPropertySetAugmenter exporter);
    }
}