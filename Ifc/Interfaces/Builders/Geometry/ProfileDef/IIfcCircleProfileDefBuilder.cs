using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcCircleProfileDefBuilder<out T> : IIfcParameterizedProfileDefBuilder<T>
        where T : IIfcCircleProfileDef
    {
        public double Radius { get; }
    }
}