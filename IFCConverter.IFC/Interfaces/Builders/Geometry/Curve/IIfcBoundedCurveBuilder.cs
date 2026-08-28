using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Curve
{
    public interface IIfcBoundedCurveBuilder<out T> : IIfcCurveBuilder<T>
        where T : IIfcBoundedCurve
    {
    }
}