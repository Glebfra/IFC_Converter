using IFCConverter.Domain;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.SegmentEntityPortResolvers
{
    internal sealed class SegmentEntityPortResolversRegistry : ReflectionRegistry<ISegmentEntityPortResolver>, ISegmentEntityPortResolversRegistry
    {
        public SegmentEntityPortResolversRegistry() : base(typeof(SegmentEntityPortResolversRegistry).Assembly)
        {
        }

        public ISegmentEntityPortResolver Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return Resolve(resolver => resolver.CanResolve(product, model, context));
        }

        public bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out ISegmentEntityPortResolver resolver)
        {
            return TryResolve(res => res.CanResolve(product, model, context), out resolver);
        }
    }
}