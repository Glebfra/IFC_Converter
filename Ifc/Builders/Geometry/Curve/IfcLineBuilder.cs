using System;
using Ifc.Extensions;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Builders.Geometry.Curve
{
    public class IfcLineBuilder<T> : IfcCurveBuilder<T>, IIfcLineBuilder<T>
        where T : IIfcLine, IInstantiableEntity
    {
        public IIfcCartesianPoint? Point { get; private set; }
        public IIfcVector? Direction { get; private set; }

        public IIfcCartesianPoint CreatePoint(IModel model, Vector<double> point)
        {
            Point = point.ToCartesianPoint(model);
            return Point;
        }

        public IIfcVector CreateDirection(IModel model, Vector<double> vector)
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