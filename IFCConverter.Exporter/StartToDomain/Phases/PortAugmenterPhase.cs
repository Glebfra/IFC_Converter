using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.PortAugmenters;
using IFCConverter.Utils.Diagnostics;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(PortResolvePhase))]
    public sealed class PortAugmenterPhase : IStartToDomainPhase
    {
        private readonly Logger _logger = Logger.GetInstance();
        private readonly IPortAugmenterRegistry _portAugmenterRegistry = new PortAugmenterRegistry();

        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context)
        {
            _logger.Info($"Starting '{nameof(PortAugmenterPhase)}'.");

            foreach (IStartEntity startEntity in source)
            {
                if (_portAugmenterRegistry.TryResolve(startEntity, out IPortAugmenter portAugmenter))
                    portAugmenter.Augment(startEntity, model, context);
            }

            _logger.Info($"Finished '{nameof(PortAugmenterPhase)}'.");
        }
    }
}