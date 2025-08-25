using Xbim.Common;
using Xbim.Ifc4.Kernel;

namespace IFC.PropertySets
{
    public interface IPropertySet
    {
        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model);
    }
}