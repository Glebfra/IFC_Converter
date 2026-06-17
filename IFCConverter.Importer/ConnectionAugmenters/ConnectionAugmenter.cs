using System.Collections.Generic;
using System.Reflection;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Interfaces;
using System.Linq;

namespace IFCConverter.Importer.ConnectionAugmenters
{
    internal sealed class ConnectionAugmenter
    {
        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity topology)
        {
            ProxyEntityAttribute attribute = topology.Proxy.Proxy.GetType().GetCustomAttribute<ProxyEntityAttribute>();
            IEntityConnectionAugmenter? augmenter = attribute.GetConnectionAugmenter();

            if (augmenter == null)
                return Enumerable.Empty<ISegmentProxy>();

            return augmenter.Augment(topology);
        }
    }
}