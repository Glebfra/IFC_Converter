using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.DomainToIfc.PropertySetAugmenters;

namespace IFCConverter.Exporter.DomainToIfc
{
    internal interface IPropertySetAugmentersRegistry
    {
        public IPropertySetAugmenter Resolve(Entity entity, ExportContext context);
        public IEnumerable<IPropertySetAugmenter> ResolveAll(Entity entity, ExportContext context);
        public bool TryResolve(Entity entity, ExportContext context, out IPropertySetAugmenter exporter);
    }
}