using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Interfaces.Geometry.Curve
{
    public interface IIfcCircleBuilder<out T> : IIfcConicBuilder<T>
        where T : IIfcCircle
    {
        IfcPositiveLengthMeasure Radius { get; }
    }
}