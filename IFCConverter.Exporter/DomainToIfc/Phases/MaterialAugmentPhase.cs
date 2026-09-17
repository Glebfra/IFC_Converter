using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.DomainToIfc.MaterialAugmenters;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.Phases
{
    [DomainToIfcPhase(1, typeof(PropertySetAugmentPhase))]
    internal sealed class MaterialAugmentPhase : IDomainToIfcPhase
    {
        private readonly IMaterialAugmentersRegistry _registry = new MaterialAugmentersRegistry();
        
        public void Execute(EngineeringModel domain, IModel model, ExportContext context)
        {
            foreach (Entity entity in domain.Entities)
            {
                if (_registry.TryResolve(entity, out IMaterialAugmenter augmenter))
                    augmenter.Augment(entity, model, context);
            }
        }
    }
}