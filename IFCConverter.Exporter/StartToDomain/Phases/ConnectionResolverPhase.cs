using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.ConnectionResolvers;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    // [StartToDomainPhase(1, typeof(PortAugmenterPhase), typeof(PortResolvePhase))]
    public sealed class ConnectionResolverPhase : IStartToDomainPhase
    {
        private readonly IConnectionResolverRegistry _connectionResolverRegistry = new ConnectionResolverRegistry();
        private readonly Logger _logger = Logger.GetInstance();
        
        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context)
        {
            _logger.Info($"Starting '{nameof(ConnectionResolverPhase)}'.");
            
            foreach (IStartEntity startEntity in source)
            {
                if (_connectionResolverRegistry.TryResolve(startEntity, out IConnectionResolver connectionResolver))
                    connectionResolver.Resolve(startEntity, model, context);
            }
            
            _logger.Info($"Finished '{nameof(ConnectionResolverPhase)}'.");
        }
    }
}