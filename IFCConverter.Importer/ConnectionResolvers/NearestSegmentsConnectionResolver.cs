using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.ConnectionResolvers
{
    public sealed class NearestSegmentsConnectionResolver : IEntityConnectionResolver
    {
        public IEnumerable<IEntityProxy> GetConnectedEntities(
            IEntityProxy proxy, 
            IReadOnlyCollection<IEntityProxy> allProxies,
            int? count = null)
        {
            IEnumerable<IEntityProxy> orderedEntities = allProxies
                .OrderBy(otherProxy => (proxy.Position - otherProxy.Position).L2Norm());
            
            return count != null ? orderedEntities.Take((int)count) : orderedEntities;
        }
    }
}