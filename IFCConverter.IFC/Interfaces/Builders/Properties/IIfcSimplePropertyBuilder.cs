using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Properties
{
    public interface IIfcSimplePropertyBuilder<out T> : IIfcPropertyBuilder<T>
        where T : IIfcSimpleProperty
    {
    }
}