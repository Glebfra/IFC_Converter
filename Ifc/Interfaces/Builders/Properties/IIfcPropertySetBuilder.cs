using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcPropertySetBuilder
    {
        public IIfcPropertySet CreatePropertySet(IModel model);
    }
}