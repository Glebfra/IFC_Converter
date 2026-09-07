using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcDiscreteAccessoryBuilder<out T> : IIfcElementBuilder<T>
        where T : IIfcDiscreteAccessory
    {
        IfcDiscreteAccessoryTypeEnum PredefinedType { get; }
    }
}