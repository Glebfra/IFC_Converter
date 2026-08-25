using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.StartToDomain.PortAugmenters;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.Phases
{
    [StartToDomainPhase(1, typeof(PortResolvePhase))]
    public sealed class PortAugmenterPhase : IStartToDomainPhase
    {
        private readonly IPortAugmenterRegistry _portAugmenterRegistry = new PortAugmenterRegistry();
        
        public void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context)
        {
            foreach (IStartEntity startEntity in source)
            {
                if (_portAugmenterRegistry.TryResolve(startEntity, out IPortAugmenter portAugmenter))
                    portAugmenter.Augment(startEntity, model, context);
            }
        }
    }
}