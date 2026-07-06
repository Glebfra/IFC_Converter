using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcPropertySingleValueBuilder<out T> : IIfcSimplePropertyBuilder<T>
        where T : IIfcPropertySingleValue
    {
        public IIfcValue NominalValue { get; }
        public IIfcUnit Unit { get; }
    }
}