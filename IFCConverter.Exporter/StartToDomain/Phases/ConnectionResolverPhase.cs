using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.ConnectionResolvers;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    // [StartToDomainPhase(1, typeof(PortAugmenterPhase), typeof(PortResolvePhase))]
    public sealed class ConnectionResolverPhase : IStartToDomainPhase
    {
        private readonly IConnectionResolverRegistry _connectionResolverRegistry = new ConnectionResolverRegistry();
        
        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context)
        {
            foreach (IStartEntity startEntity in source)
            {
                if (_connectionResolverRegistry.TryResolve(startEntity, out IConnectionResolver connectionResolver))
                    connectionResolver.Resolve(startEntity, model, context);
            }
        }
    }
}