using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Interfaces.Geometry.SolidModel
{
    public interface IIfcSurfaceCurveSweptAreaSolidBuilder<out T> : IIfcSweptAreaSolidBuilder<T>
        where T : IIfcSurfaceCurveSweptAreaSolid
    {
        IIfcCurve Directrix { get; }
        IIfcSurface ReferenceSurface { get; }

        IfcParameterValue StartParam { get; }
        IfcParameterValue EndParam { get; }
    }
}