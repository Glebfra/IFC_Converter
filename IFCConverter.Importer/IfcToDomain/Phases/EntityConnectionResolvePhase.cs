using System;
using IFCConverter.Domain;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.IfcToDomain.EntityConnectionsResolvers;
using Microsoft.Extensions.Logging;
using Xbim.Common;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    [IfcToDomainPhase(1, typeof(SegmentSplitPhase))]
    internal sealed class EntityConnectionResolvePhase : IIfcToDomainPhase
    {
        private readonly IEntityConnectionResolversRegistry _registry = new EntityConnectionsResolversRegistry();

        public void Execute(IModel model, EngineeringModel domain, ImportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<EntityConnectionResolvePhase>();

            foreach (IEntityConnectionResolver resolver in _registry.ResolveAll())
            {
                try
                {
                    logger.LogInformation("Found entity connection resolver {resolver}", resolver.GetType().Name);
                    resolver.Resolve(domain, context);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                }
            }
        }
    }
}