using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcSimplePropertyBuilder<out T> : IIfcPropertyBuilder<T>
        where T : IIfcSimpleProperty
    {
    }
}