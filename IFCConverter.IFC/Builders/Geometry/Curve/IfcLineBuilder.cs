using System;
using IFCConverter.IFC.Extensions;
using IFCConverter.IFC.Interfaces.Geometry.Curve;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Builders.Geometry.Curve
{
    public class IfcLineBuilder<T> : IfcCurveBuilder<T>, IIfcLineBuilder<T>
        where T : IIfcLine, IInstantiableEntity
    {
        public IIfcCartesianPoint Point { get; private set; }
        public IIfcVector Direction { get; private set; }

        public IIfcCartesianPoint CreatePoint(IModel model, FixedVector<Dim3> point)
        {
            Point = point.ToCartesianPoint(model);
            return Point;
        }

        public IIfcVector CreateDirection(IModel model, FixedVector<Dim3> vector)
        {
            Direction = vector.ToIfcVector(model);
            return Direction;
        }

        public override T CreateCurve(IModel model)
        {
            if (Point == null)
                throw new NullReferenceException(
                    $"{nameof(IfcLineBuilder<T>)}: {nameof(Point)}. Call {nameof(CreatePoint)} before {nameof(CreateCurve)}");
            if (Direction == null)
                throw new NullReferenceException(
                    $"{nameof(IfcLineBuilder<T>)}: {nameof(Direction)}. Call {nameof(CreateDirection)} before {nameof(CreateCurve)}");

            T curve = base.CreateCurve(model);
            curve.Pnt = Point;
            curve.Dir = Direction;
            return curve;
        }
    }
}