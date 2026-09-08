using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.ProfileDef
{
    public interface IIfcCircleHollowProfileDefBuilder<T> : IIfcCircleProfileDefBuilder<T>
        where T : IIfcCircleHollowProfileDef
    {
        double WallThickness { get; }
    }
}