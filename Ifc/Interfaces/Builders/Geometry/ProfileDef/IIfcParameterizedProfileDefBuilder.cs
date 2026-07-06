using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcParameterizedProfileDefBuilder<out T> : IIfcProfileDefBuilder<T>
        where T : IIfcParameterizedProfileDef
    {
        public IIfcAxis2Placement2D? Position { get; }

        public IIfcAxis2Placement2D CreatePosition(IModel model, Matrix<double> matrix);
    }
}