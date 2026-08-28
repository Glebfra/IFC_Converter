using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Curve
{
    public interface IIfcLineBuilder<out T> : IIfcCurveBuilder<T>
        where T : IIfcLine
    {
        IIfcCartesianPoint Point { get; }
        IIfcVector Direction { get; }

        IIfcCartesianPoint CreatePoint(IModel model, Vector<double> point);
        IIfcVector CreateDirection(IModel model, Vector<double> vector);
    }
}