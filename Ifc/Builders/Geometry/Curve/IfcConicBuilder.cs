using System;
using Ifc.Extensions;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Builders.Geometry.Curve
{
    public class IfcConicBuilder<T> : IfcCurveBuilder<T>, IIfcConicBuilder<T>
        where T : IIfcConic, IInstantiableEntity
    {
        public IIfcAxis2Placement2D? Position { get; private set; }

        public IIfcAxis2Placement2D CreatePosition(IModel model, Matrix<double> matrix)
        {
            Position = matrix.ToAxis2Placement2D(model);
            return Position;
        }

        public override T CreateCurve(IModel model)
        {
            if (Position == null)
                throw new NullReferenceException(
                    $"{nameof(IfcConicBuilder<T>)}: {nameof(Position)}. Call {nameof(CreatePosition)} before {nameof(CreateCurve)}");

            T curve = base.CreateCurve(model);
            curve.Position = Position;
            return curve;
        }
    }
}