using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IConnectionResolver
    {
        IEnumerable<IBoundaryProxy> GetConnectedEntities(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> allProxies);
    }
}