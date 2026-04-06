using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcDistributionFlowElementBuilder<out T> : IIfcDistributionElementBuilder<T>
        where T : IIfcDistributionFlowElement
    {
    }
}