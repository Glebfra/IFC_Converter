using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IEntityConnectionAugmenter
    {
        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity topology);
    }
}