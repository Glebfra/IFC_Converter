using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Importer.IfcToDomain.EntityPortResolvers.SegmentEntityPortResolvers;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers
{
    internal sealed class SegmentEntityPortResolver : IEntityPortResolver
    {
        private readonly ISegmentEntityPortResolversRegistry _registry = new SegmentEntityPortResolversRegistry();
        
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out EntityId id))
                return false;

            Entity entity = model.GetEntity(id);
            return entity is Segment;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, model, context, out ISegmentEntityPortResolver resolver))
                resolver.Resolve(product, model, context);
        }
    }
}