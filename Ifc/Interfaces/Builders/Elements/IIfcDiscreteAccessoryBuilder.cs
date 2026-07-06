using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcDiscreteAccessoryBuilder<out T> : IIfcElementBuilder<T>
        where T : IIfcDiscreteAccessory
    {
        public IfcDiscreteAccessoryTypeEnum PredefinedType { get; }
    }
}