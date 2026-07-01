using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;

namespace IFCConverter.Importer.Topology
{
    internal static class TopologyEntityFactory
    {
        public static ITopologyEntity CreateTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes)
        {
            return proxy.Proxy switch
            {
                ValveProxy => new ValveTopologyEntity(proxy, nodes),
                PipeSegmentProxy => new SegmentTopologyEntity(proxy, nodes),
                _ => new TopologyEntity(proxy, nodes)
            };
        }
    }
}