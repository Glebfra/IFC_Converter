using System.Linq;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.TopologyModelAugmenter
{
    internal sealed class SegmentAugmentableModelAugmenter : ITopologyModelAugmenter
    {
        public void Augment(ITopologyModel model)
        {
            foreach (ISegmentAugmentableTopologyEntity segmentAugmentableTopologyEntity in model.Entities.OfType<ISegmentAugmentableTopologyEntity>())
            {
                segmentAugmentableTopologyEntity.Augment();
            }
        }
    }
}