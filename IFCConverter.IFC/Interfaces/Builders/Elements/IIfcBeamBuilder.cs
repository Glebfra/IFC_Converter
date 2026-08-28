using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcBeamBuilder<T>
    {
        IfcBeamTypeEnum PredefinedType { get; }
    }
}