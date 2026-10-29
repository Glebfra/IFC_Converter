using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.StartToDomain.ConnectionResolvers;
using IFCConverter.Start.Interfaces;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    // [StartToDomainPhase(1, typeof(PortAugmenterPhase), typeof(PortResolvePhase))]
    public sealed class ConnectionResolverPhase : IStartToDomainPhase
    {
        private readonly IConnectionResolverRegistry _connectionResolverRegistry = new ConnectionResolverRegistry();

        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<ConnectionResolverPhase>();

            foreach (IStartEntity startEntity in source)
            {
                if (_connectionResolverRegistry.TryResolve(startEntity, out IConnectionResolver connectionResolver))
                {
                    logger.LogInformation(
                        "Found connection resolver {resolver} for start entity {entity} with name {name}",
                        connectionResolver.GetType().Name,
                        startEntity.GetType().Name,
                        startEntity.Name
                    );
                    connectionResolver.Resolve(startEntity, model, context);
                }
            }
        }
    }
}