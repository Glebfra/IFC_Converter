using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Builders.Geometry.Curve
{
    public class IfcBoundedCurveBuilder<T> : IfcCurveBuilder<T>, IIfcBoundedCurveBuilder<T>
        where T : IIfcBoundedCurve, IInstantiableEntity
    {
    }
}