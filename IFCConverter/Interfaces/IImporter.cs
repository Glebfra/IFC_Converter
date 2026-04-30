using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Start.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Interfaces
{
    public interface IImporter
    {
        [Pure]
        public IEnumerable<IStartEntity> ImportEntities(IEnumerable<IfcProduct> products);
    }
}