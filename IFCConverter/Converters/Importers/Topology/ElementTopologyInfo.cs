using System.Collections.Generic;
using IFCConverter.Interfaces;

namespace IFCConverter.Converters.Importers.Topology
{
    internal sealed class ElementTopologyInfo
    {
        public IEntityProxy Proxy { get; }
        public IReadOnlyList<TopologyNode> Nodes { get; }
        
        public ElementTopologyInfo(IEntityProxy proxy, IReadOnlyList<TopologyNode> nodes)
        {
            Proxy = proxy;
            Nodes = nodes;
        }
    }
}