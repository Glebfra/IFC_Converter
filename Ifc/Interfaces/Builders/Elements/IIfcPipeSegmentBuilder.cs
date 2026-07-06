using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcPipeSegmentBuilder<out T> : IIfcFlowSegmentBuilder<T>
        where T : IIfcPipeSegment
    {
        public IfcPipeSegmentTypeEnum PipeSegmentTypeEnum { get; }
    }
}