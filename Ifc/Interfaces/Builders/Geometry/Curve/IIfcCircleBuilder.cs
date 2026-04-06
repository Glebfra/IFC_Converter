using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Interfaces
{
    public interface IIfcCircleBuilder<out T> : IIfcConicBuilder<T>
        where T : IIfcCircle
    {
        public IfcPositiveLengthMeasure Radius { get; }
    }
}