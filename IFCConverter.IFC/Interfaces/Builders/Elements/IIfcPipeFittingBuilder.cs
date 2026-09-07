using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcPipeFittingBuilder<out T> : IIfcFlowFittingBuilder<T>
        where T : IIfcPipeFitting
    {
        IfcPipeFittingTypeEnum PipeFittingType { get; }
    }
}