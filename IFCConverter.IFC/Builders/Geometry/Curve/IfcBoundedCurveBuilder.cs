using IFCConverter.IFC.Interfaces.Geometry.Curve;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Builders.Geometry.Curve
{
    public class IfcBoundedCurveBuilder<T> : IfcCurveBuilder<T>, IIfcBoundedCurveBuilder<T>
        where T : IIfcBoundedCurve, IInstantiableEntity
    {
    }
}