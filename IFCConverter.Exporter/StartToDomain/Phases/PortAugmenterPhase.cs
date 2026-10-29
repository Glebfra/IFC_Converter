using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.PortAugmenters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Diagnostics;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(PortResolvePhase))]
    public sealed class PortAugmenterPhase : IStartToDomainPhase
    {
        private readonly IPortAugmenterRegistry _portAugmenterRegistry = new PortAugmenterRegistry();

        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<PortAugmenterPhase>();

            foreach (IStartEntity startEntity in source)
            {
                if (_portAugmenterRegistry.TryResolve(startEntity, out IPortAugmenter portAugmenter))
                {
                    logger.LogInformation(
                        "Found port augmenter {augmenter} for start entity {entity} with name {name}",
                        portAugmenter.GetType().Name,
                        startEntity.GetType().Name,
                        startEntity.Name
                    );
                    portAugmenter.Augment(startEntity, model, context);
                }
            }
        }
    }
}