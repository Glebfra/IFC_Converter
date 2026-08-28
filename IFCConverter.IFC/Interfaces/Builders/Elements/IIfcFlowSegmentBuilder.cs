using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcFlowSegmentBuilder<out T> : IIfcDistributionFlowElementBuilder<T>
        where T : IIfcFlowSegment
    {
    }
}