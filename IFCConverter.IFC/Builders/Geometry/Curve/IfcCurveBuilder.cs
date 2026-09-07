using IFCConverter.IFC.Interfaces.Geometry.Curve;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Builders.Geometry.Curve
{
    public class IfcCurveBuilder<T> : IIfcCurveBuilder<T>
        where T : IIfcCurve, IInstantiableEntity
    {
        public object Instance => IfcCurve;

        public T IfcCurve { get; private set; }

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