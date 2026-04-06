using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcPolylineBuilder<out T> : IIfcBoundedCurveBuilder<T>
        where T : IIfcPolyline
    {
        public IEnumerable<IIfcCartesianPoint>? Points { get; }

        public void CreatePoints(IModel model, IEnumerable<Vector<double>> points);
    }
}