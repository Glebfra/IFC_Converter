using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcFlowFittingBuilder<out T> : IIfcDistributionFlowElementBuilder<T>
        where T : IIfcFlowFitting
    {
    }
}