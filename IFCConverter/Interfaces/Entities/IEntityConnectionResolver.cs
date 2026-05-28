using System.Collections.Generic;

namespace IFCConverter.Interfaces
{
    internal interface IEntityConnectionResolver
    {
        IEnumerable<IEntityProxy> GetConnectedEntities(
            IEntityProxy proxy,
            IReadOnlyCollection<IEntityProxy> allProxies,
            int? count = null);
    }
}