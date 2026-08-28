using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;

namespace IFCConverter.Importer.Topology
{
    internal static class TopologyEntityFactory
    {
        public static ITopologyEntity CreateTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes)
        {
            switch (proxy.Proxy)
            {
                case ValveProxy _:
                    return new ValveTopologyEntity(proxy, nodes);
                case PipeSegmentProxy _:
                    return new SegmentTopologyEntity(proxy, nodes);
                default:
                    return new TopologyEntity(proxy, nodes);
            }
        }
    }
}