using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.PortResolvers;
using IFCConverter.Start.Interfaces;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(EntityMetadataAugmentPhase))]
    public sealed class PortResolvePhase : IStartToDomainPhase
    {
        private readonly IPortResolverRegistry _portResolverRegistry = new PortResolverRegistry();

        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<PortResolvePhase>();

            foreach (IStartEntity startEntity in source)
            {
                if (_portResolverRegistry.TryResolve(startEntity, out IPortResolver portResolver))
                {
                    logger.LogInformation(
                        "Found port resolver {resolver} for start entity {entity} with name {name}",
                        portResolver.GetType().Name,
                        startEntity.GetType().Name,
                        startEntity.Name
                    );
                    portResolver.Resolve(startEntity, model, context);
                }
            }
        }
    }
}