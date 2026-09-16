using IFCConverter.Domain;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.IfcToDomain.EntityConnectionsResolvers;
using Xbim.Common;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    [IfcToDomainPhase(1, typeof(EntityPortResolvePhase))]
    internal sealed class EntityConnectionResolvePhase : IIfcToDomainPhase
    {
        private readonly IEntityConnectionResolversRegistry _registry = new EntityConnectionsResolversRegistry();
        
        public void Execute(IModel model, EngineeringModel domain, ImportContext context)
        {
            foreach (IEntityConnectionResolver resolver in _registry.ResolveAll())
                resolver.Resolve(domain, context);
        }
    }
}