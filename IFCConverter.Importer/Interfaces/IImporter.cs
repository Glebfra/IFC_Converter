using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.Interfaces
{
    public interface IImporter
    {
        [Pure]
        public IReadOnlyCollection<ITopologyEntity> ImportEntities(IEnumerable<IfcProduct> products);
    }
}