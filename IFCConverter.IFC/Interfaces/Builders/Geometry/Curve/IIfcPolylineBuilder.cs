using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Curve
{
    public interface IIfcPolylineBuilder<out T> : IIfcBoundedCurveBuilder<T>
        where T : IIfcPolyline
    {
        IEnumerable<IIfcCartesianPoint> Points { get; }

        void CreatePoints(IModel model, IEnumerable<FixedVector<Dim3>> points);
    }
}