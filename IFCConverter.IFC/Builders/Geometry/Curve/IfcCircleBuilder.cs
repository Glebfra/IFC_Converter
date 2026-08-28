using IFCConverter.IFC.Interfaces.Geometry.Curve;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Builders.Geometry.Curve
{
    public class IfcCircleBuilder<T> : IfcConicBuilder<T>, IIfcCircleBuilder<T>
        where T : IIfcCircle, IInstantiableEntity
    {
        public IfcCircleBuilder(IfcPositiveLengthMeasure radius)
        {
            Radius = radius;
        }

        public IfcPositiveLengthMeasure Radius { get; }

        public override T CreateCurve(IModel model)
        {
            T curve = base.CreateCurve(model);
            curve.Radius = Radius;
            return curve;
        }
    }
}