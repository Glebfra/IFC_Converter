using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologyAugmenter
    {
        public IReadOnlyCollection<ITopologyEntity> Augment(IReadOnlyCollection<ITopologyEntity> topologies);
    }
}