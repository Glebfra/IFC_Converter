using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.ProfileDef
{
    public interface IIfcRectangleProfileDefBuilder<out T> : IIfcParameterizedProfileDefBuilder<T>
        where T : IIfcRectangleProfileDef
    {
        double XDim { get; }
        double YDim { get; }
    }
}