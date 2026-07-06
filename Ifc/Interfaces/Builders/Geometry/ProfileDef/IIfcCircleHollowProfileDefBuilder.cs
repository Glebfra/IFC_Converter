using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcCircleHollowProfileDefBuilder<T> : IIfcCircleProfileDefBuilder<T>
        where T : IIfcCircleHollowProfileDef
    {
        public double WallThickness { get; }
    }
}