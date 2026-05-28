using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface INodeTopologyResolver
    {
        public IReadOnlyCollection<ITopologyNodeEntity> ResolveTopology(
            IEntityProxy proxy, 
            IReadOnlyCollection<IEntityProxy> connectedProxies);
    }
}