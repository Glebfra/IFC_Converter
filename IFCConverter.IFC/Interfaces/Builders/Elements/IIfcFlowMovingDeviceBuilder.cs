using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcFlowMovingDeviceBuilder<out T> : IIfcDistributionFlowElementBuilder<T>
        where T : IIfcFlowMovingDevice
    {
        
    }
}