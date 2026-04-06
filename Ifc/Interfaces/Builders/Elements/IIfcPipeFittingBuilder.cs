using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcPipeFittingBuilder<out T> : IIfcFlowFittingBuilder<T>
        where T : IIfcPipeFitting
    {
        public IfcPipeFittingTypeEnum PipeFittingType { get; }
    }
}