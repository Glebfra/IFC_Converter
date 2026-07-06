using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcRectangleProfileDefBuilder<out T> : IIfcParameterizedProfileDefBuilder<T>
        where T : IIfcRectangleProfileDef
    {
        public double XDim { get; }
        public double YDim { get; }
    }
}