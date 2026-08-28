using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Curve
{
    public interface IIfcConicBuilder<out T> : IIfcCurveBuilder<T>
        where T : IIfcConic
    {
        IIfcAxis2Placement2D Position { get; }

        IIfcAxis2Placement2D CreatePosition(IModel model, Matrix<double> matrix);
    }
}