using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Curve
{
    public interface IIfcConicBuilder<out T> : IIfcCurveBuilder<T>
        where T : IIfcConic
    {
        IIfcAxis2Placement2D Position { get; }

        IIfcAxis2Placement2D CreatePosition(IModel model, FixedMatrix<Dim4> matrix);
    }
}