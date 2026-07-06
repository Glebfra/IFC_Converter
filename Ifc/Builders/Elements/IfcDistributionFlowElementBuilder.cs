using Ifc.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace Ifc.Builders.Elements
{
    public class IfcDistributionFlowElementBuilder<T> : IfcDistributionElementBuilder<T>,
        IIfcDistributionFlowElementBuilder<T>
        where T : IfcDistributionFlowElement
    {
        public IfcDistributionFlowElementBuilder(IfcLabel name, IfcIdentifier tag) : base(name, tag)
        {
        }
    }
}