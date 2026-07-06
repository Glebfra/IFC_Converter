using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcBoundedCurveBuilder<out T> : IIfcCurveBuilder<T>
        where T : IIfcBoundedCurve
    {
    }
}