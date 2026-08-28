using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcDistributionFlowElementBuilder<out T> : IIfcDistributionElementBuilder<T>
        where T : IIfcDistributionFlowElement
    {
    }
}