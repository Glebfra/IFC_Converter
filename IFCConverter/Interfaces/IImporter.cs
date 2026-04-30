using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Start.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Interfaces
{
    internal interface IImporter
    {
        [Pure]
        public IEnumerable<IEntityProxy> ImportEntities(IEnumerable<IfcProduct> products);
    }
}