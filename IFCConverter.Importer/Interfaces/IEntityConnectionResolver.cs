using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IEntityConnectionResolver
    {
        [Pure]
        public IEnumerable<IBoundaryProxy> GetConnectedEntities(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> allProxies);
    }
}