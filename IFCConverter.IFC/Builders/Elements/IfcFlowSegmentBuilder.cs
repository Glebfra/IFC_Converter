using IFCConverter.IFC.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFCConverter.IFC.Builders.Elements
{
    public class IfcFlowSegmentBuilder<T> : IfcDistributionFlowElementBuilder<T>, IIfcFlowSegmentBuilder<T>
        where T : IfcFlowSegment
    {
        public IfcFlowSegmentBuilder(IfcLabel name, IfcIdentifier tag) : base(name, tag)
        {
        }
    }
}