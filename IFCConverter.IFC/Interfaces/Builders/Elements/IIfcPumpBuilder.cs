using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcPumpBuilder<out T> : IIfcFlowMovingDeviceBuilder<T>
        where T : IIfcPump
    {
        IfcPumpTypeEnum PredefinedType { get; }
    }
}