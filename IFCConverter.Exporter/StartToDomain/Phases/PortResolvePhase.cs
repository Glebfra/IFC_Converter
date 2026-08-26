using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.PortResolvers;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(EntityMetadataAugmentPhase))]
    public sealed class PortResolvePhase : IStartToDomainPhase
    {
        private readonly IPortResolverRegistry _portResolverRegistry = new PortResolverRegistry();
        private readonly Logger _logger = Logger.GetInstance();
        
        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context)
        {
            _logger.Info($"Starting '{nameof(PortResolvePhase)}'.");
            
            foreach (IStartEntity startEntity in source)
            {
                if (_portResolverRegistry.TryResolve(startEntity, out IPortResolver portResolver))
                    portResolver.Resolve(startEntity, model, context);
            }
            
            _logger.Info($"Finished '{nameof(PortResolvePhase)}'.");
        }
    }
}