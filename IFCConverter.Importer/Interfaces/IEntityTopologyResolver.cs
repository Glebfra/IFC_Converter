using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    public interface IEntityTopologyResolver
    {
        public ITopologyEntity ResolveTopology(IEntityProxy proxy, IReadOnlyCollection<IEntityProxy> allProxies);
    }
}