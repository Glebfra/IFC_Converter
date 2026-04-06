using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcLineBuilder<out T> : IIfcCurveBuilder<T>
        where T : IIfcLine
    {
        public IIfcCartesianPoint? Point { get; }
        public IIfcVector? Direction { get; }

        public IIfcCartesianPoint CreatePoint(IModel model, Vector<double> point);
        public IIfcVector CreateDirection(IModel model, Vector<double> vector);
    }
}