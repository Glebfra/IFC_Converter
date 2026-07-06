using Ifc.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace Ifc.Builders.Elements
{
    public class IfcFlowElementBuilder<T> : IfcDistributionFlowElementBuilder<T>, IIfcFlowFittingBuilder<T>
        where T : IfcFlowFitting
    {
        public IfcFlowElementBuilder(IfcLabel name, IfcIdentifier tag) : base(name, tag)
        {
        }
    }
}