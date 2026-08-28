using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.PortResolvers;
using IFCConverter.Utils.Diagnostics;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(EntityMetadataAugmentPhase))]
    public sealed class PortResolvePhase : IStartToDomainPhase
    {
        private readonly Logger _logger = Logger.GetInstance();
        private readonly IPortResolverRegistry _portResolverRegistry = new PortResolverRegistry();

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