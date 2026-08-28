using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcPipeSegmentBuilder<out T> : IIfcFlowSegmentBuilder<T>
        where T : IIfcPipeSegment
    {
        IfcPipeSegmentTypeEnum PipeSegmentTypeEnum { get; }
    }
}