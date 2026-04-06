using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Interfaces
{
    public interface IIfcRevolvedAreaSolidBuilder<out T> : IIfcSweptAreaSolidBuilder<T>
        where T : IIfcRevolvedAreaSolid
    {
        public IIfcAxis1Placement? Axis { get; }
        public IfcPlaneAngleMeasure Angle { get; }

        public IIfcAxis1Placement CreateAxis(IModel model, Vector<double> axisPosition, Vector<double> axisDirection);
    }
}