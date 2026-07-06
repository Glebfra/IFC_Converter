using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcConicBuilder<out T> : IIfcCurveBuilder<T>
        where T : IIfcConic
    {
        public IIfcAxis2Placement2D? Position { get; }

        public IIfcAxis2Placement2D CreatePosition(IModel model, Matrix<double> matrix);
    }
}