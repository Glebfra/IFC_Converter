using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.IfcToDomain.SegmentSplitters;
using Xbim.Common;

namespace IFCConverter.Importer.IfcToDomain.Phases
{
    [IfcToDomainPhase(1, typeof(EntityMetadataAugmentPhase))]
    public sealed class SegmentSplitPhase : IIfcToDomainPhase
    {
        private readonly ISegmentSplittersRegistry _registry = new SegmentSplittersRegistry();
        
        public void Execute(IModel model, EngineeringModel domain, ImportContext context)
        {
            foreach (Entity entity in domain.Entities.ToArray())
            {
                if (_registry.TryResolve(entity, domain, context, out ISegmentSplitter splitter))
                    splitter.Split(entity, domain, context);
            }
        }
    }
}