using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.PortResolvers;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(EntityMetadataAugmentPhase))]
    public sealed class PortResolvePhase : IStartToDomainPhase
    {
        private readonly IPortResolverRegistry _portResolverRegistry = new PortResolverRegistry();
        
        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context)
        {
            foreach (IStartEntity startEntity in source)
            {
                if (_portResolverRegistry.TryResolve(startEntity, out IPortResolver portResolver))
                    portResolver.Resolve(startEntity, model, context);
            }
        }
    }
}