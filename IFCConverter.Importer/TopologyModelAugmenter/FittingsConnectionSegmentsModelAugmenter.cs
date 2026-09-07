using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.TopologyModelAugmenter
{
    internal sealed class FittingsConnectionSegmentsModelAugmenter : ITopologyModelAugmenter
    {
        public void Augment(ITopologyModel model)
        {
            List<ISegmentProxy> result = new List<ISegmentProxy>();
            foreach (IFittingTopologyEntity fittingTopologyEntity in model.Entities.OfType<IFittingTopologyEntity>().ToArray())
            {
                TopologyEntityAttribute attribute = fittingTopologyEntity.GetTopologyEntityAttribute();
                ITopologySegmentAugmenter augmenter = attribute.GetSegmentAugmenter();
                if (augmenter == null)
                    continue;

                result.AddRange(augmenter.Augment(fittingTopologyEntity));
            }

            model.AddEntities(result);
        }
    }
}