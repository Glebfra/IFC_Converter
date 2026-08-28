using IFCConverter.IFC.Interfaces.Properties;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Builders.Properties
{
    public class IfcSimplePropertyBuilder<T> : IfcPropertyBuilder<T>, IIfcSimplePropertyBuilder<T>
        where T : IIfcSimpleProperty, IInstantiableEntity
    {
        public IfcSimplePropertyBuilder(IfcIdentifier name, IfcText description)
            : base(name, description)
        {
        }
    }
}