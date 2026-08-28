using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcElementBuilder<out T> : IIfcProductBuilder<T>
        where T : IIfcElement
    {
        IfcIdentifier Tag { get; }
    }
}