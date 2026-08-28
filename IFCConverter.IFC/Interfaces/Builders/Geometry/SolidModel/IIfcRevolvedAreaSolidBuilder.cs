using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Interfaces.Geometry.SolidModel
{
    public interface IIfcRevolvedAreaSolidBuilder<out T> : IIfcSweptAreaSolidBuilder<T>
        where T : IIfcRevolvedAreaSolid
    {
        IIfcAxis1Placement Axis { get; }
        IfcPlaneAngleMeasure Angle { get; }

        IIfcAxis1Placement CreateAxis(IModel model, Vector<double> axisPosition, Vector<double> axisDirection);
    }
}