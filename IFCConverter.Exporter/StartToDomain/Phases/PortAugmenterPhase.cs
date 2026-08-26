using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.PortAugmenters;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(PortResolvePhase))]
    public sealed class PortAugmenterPhase : IStartToDomainPhase
    {
        private readonly IPortAugmenterRegistry _portAugmenterRegistry = new PortAugmenterRegistry();
        private readonly Logger _logger = Logger.GetInstance();
        
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