using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Interfaces
{
    public interface IIfcSurfaceCurveSweptAreaSolidBuilder<out T> : IIfcSweptAreaSolidBuilder<T>
        where T : IIfcSurfaceCurveSweptAreaSolid
    {
        public IIfcCurve Directrix { get; }
        public IIfcSurface ReferenceSurface { get; }

        public IfcParameterValue StartParam { get; }
        public IfcParameterValue EndParam { get; }
    }
}