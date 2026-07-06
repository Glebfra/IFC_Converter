using Ifc.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace Ifc.Builders.Elements
{
    public class IfcFlowSegmentBuilder<T> : IfcDistributionFlowElementBuilder<T>, IIfcFlowSegmentBuilder<T>
        where T : IfcFlowSegment
    {
        public IfcFlowSegmentBuilder(IfcLabel name, IfcIdentifier tag) : base(name, tag)
        {
        }
    }
}