using System.Collections.Generic;

namespace IFCConverter.Interfaces
{
    internal interface IEntityTopologyResolver
    {
        public ITopologyEntity ResolveTopology(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies);
    }
}