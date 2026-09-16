using IFCConverter.IFC.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFCConverter.IFC.Builders.Elements
{
    public class IfcFlowMovingDeviceBuilder<T> : IfcDistributionFlowElementBuilder<T>, IIfcFlowMovingDeviceBuilder<T>
        where T : IfcFlowMovingDevice
    {
        public IfcFlowMovingDeviceBuilder(IfcLabel name, IfcIdentifier tag) : base(name, tag)
        {
        }
    }
}