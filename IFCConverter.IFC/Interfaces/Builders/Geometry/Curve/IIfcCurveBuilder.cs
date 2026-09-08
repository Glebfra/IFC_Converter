using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Curve
{
    public interface IIfcCurveBuilder<out T> : IIfcBuilder
        where T : IIfcCurve
    {
        T IfcCurve { get; }

        T CreateCurve(IModel model);
    }
}