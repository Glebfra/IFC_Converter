using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Curve
{
    public interface IIfcLineBuilder<out T> : IIfcCurveBuilder<T>
        where T : IIfcLine
    {
        IIfcCartesianPoint Point { get; }
        IIfcVector Direction { get; }

        IIfcCartesianPoint CreatePoint(IModel model, FixedVector<Dim3> point);
        IIfcVector CreateDirection(IModel model, FixedVector<Dim3> vector);
    }
}