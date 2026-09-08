using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IImporter
    {
        [Pure]
        IReadOnlyCollection<IEntityProxy> ImportProxies(IEnumerable<IfcProduct> products);
    }
}