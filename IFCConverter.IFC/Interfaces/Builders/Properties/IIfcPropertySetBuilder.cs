using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Properties
{
    public interface IIfcPropertySetBuilder
    {
        IIfcPropertySet CreatePropertySet(IModel model);
    }
}