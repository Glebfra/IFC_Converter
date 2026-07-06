using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Builders.Geometry.Curve
{
    public class IfcCurveBuilder<T> : IIfcCurveBuilder<T>
        where T : IIfcCurve, IInstantiableEntity
    {
        public object? Instance => IfcCurve;

        public T? IfcCurve { get; private set; }

        public virtual T CreateCurve(IModel model)
        {
            IfcCurve = model.Instances.New<T>();
            return IfcCurve;
        }

        public object Build(IModel model)
        {
            return CreateCurve(model);
        }
    }
}