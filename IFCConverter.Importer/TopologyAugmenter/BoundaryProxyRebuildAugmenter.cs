using System;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.TopologyAugmenter
{
    internal sealed class BoundaryProxyRebuildAugmenter : ITopologyAugmenter
    {
        public ITopologyModel Augment(ITopologyModel model)
        {
            foreach (ITopologyEntity topologyEntity in model.Entities)
            {

            }

            throw new NotImplementedException();
        }
    }
}