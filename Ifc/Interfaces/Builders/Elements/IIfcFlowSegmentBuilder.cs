using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcFlowSegmentBuilder<out T> : IIfcDistributionFlowElementBuilder<T>
        where T : IIfcFlowSegment
    {
    }
}