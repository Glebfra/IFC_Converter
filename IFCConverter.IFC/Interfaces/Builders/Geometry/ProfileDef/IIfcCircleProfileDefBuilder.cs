using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.ProfileDef
{
    public interface IIfcCircleProfileDefBuilder<out T> : IIfcParameterizedProfileDefBuilder<T>
        where T : IIfcCircleProfileDef
    {
        double Radius { get; }
    }
}