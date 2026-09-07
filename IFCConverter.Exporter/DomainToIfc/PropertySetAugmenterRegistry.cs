using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.DomainToIfc.PropertySetAugmenters;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.DomainToIfc
{
    internal sealed class PropertySetAugmenterRegistry : ReflectionRegistry<IPropertySetAugmenter>, IPropertySetAugmentersRegistry
    {
        public PropertySetAugmenterRegistry() : base(typeof(PropertySetAugmenterRegistry).Assembly)
        {
        }

        public IPropertySetAugmenter Resolve(Entity entity, ExportContext context)
        {
            return Resolve(augmenter => augmenter.CanAugment(entity, context));
        }

        public IEnumerable<IPropertySetAugmenter> ResolveAll(Entity entity, ExportContext context)
        {
            return ResolveAll(augmenter => augmenter.CanAugment(entity, context));
        }

        public bool TryResolve(Entity entity, ExportContext context, out IPropertySetAugmenter exporter)
        {
            return TryResolve(augmenter => augmenter.CanAugment(entity, context), out exporter);
        }
    }
}