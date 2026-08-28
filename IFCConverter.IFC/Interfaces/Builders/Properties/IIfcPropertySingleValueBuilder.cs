using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Properties
{
    public interface IIfcPropertySingleValueBuilder<out T> : IIfcSimplePropertyBuilder<T>
        where T : IIfcPropertySingleValue
    {
        IIfcValue NominalValue { get; }
        IIfcUnit Unit { get; }
    }
}