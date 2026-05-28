using System.Collections.Generic;

namespace IFCConverter.Interfaces
{
    internal interface INodeTopologyResolver
    {
        public IReadOnlyCollection<ITopologyNodeEntity> ResolveTopology(
            IEntityProxy proxy, 
            IReadOnlyCollection<IEntityProxy> connectedProxies);
    }
}