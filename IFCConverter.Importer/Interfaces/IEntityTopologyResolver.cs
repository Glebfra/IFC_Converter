using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IEntityTopologyResolver
    {
        public IReadOnlyCollection<ITopologyEntity> ResolveTopologies(IReadOnlyCollection<IEntityProxy> allProxies);
    }
}