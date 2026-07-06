using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IConnectionResolver
    {
        public IEnumerable<IBoundaryProxy> GetConnectedEntities(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> allProxies);
    }
}