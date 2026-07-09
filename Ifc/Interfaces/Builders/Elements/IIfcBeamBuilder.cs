using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcBeamBuilder<T>
    {
        public IfcBeamTypeEnum PredefinedType { get; }
    }
}