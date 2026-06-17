using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface INodeTopologyResolver
    {
        public IEnumerable<ITopologyNodeEntity> ResolveTopology(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> connected);
    }
}