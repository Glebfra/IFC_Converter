using System.Collections.Generic;
using Start.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Interfaces
{
    public interface IImporter
    {
        public IEnumerable<IStartEntity> ImportEntities(IEnumerable<IfcProduct> products);
    }
}