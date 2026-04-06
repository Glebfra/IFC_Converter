using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Builders.Properties
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