using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcBeamBuilder<out T> : IIfcElementBuilder<T>
        where T : IIfcBeam
    {
        IfcBeamTypeEnum PredefinedType { get; }
    }
}