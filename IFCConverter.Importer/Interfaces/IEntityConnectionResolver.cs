using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    public interface IEntityConnectionResolver
    {
        IEnumerable<IEntityProxy> GetConnectedEntities(
            IEntityProxy proxy,
            IReadOnlyCollection<IEntityProxy> allProxies,
            int? count = null);
    }
}