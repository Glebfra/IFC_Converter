using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace IFCConverter.Importer.Interfaces
{
    internal interface INodeTopologyResolver
    {
        [Pure]
        public IEnumerable<ITopologyNodeEntity> ResolveTopologyRaw(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> connected);
        
        [Pure]
        public IEnumerable<ITopologyNodeEntity> ResolveTopology(IBoundaryProxy proxy, IEnumerable<IBoundaryProxy> connected);
    }
}