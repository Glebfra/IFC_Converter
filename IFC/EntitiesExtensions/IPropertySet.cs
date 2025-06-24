using Xbim.Common;
using Xbim.Ifc4.Kernel;

namespace IFC.EntitiesExtensions
{
    public interface IPropertySet
    {
        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model);
    }
}