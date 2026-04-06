using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Builders.Geometry.SolidModel
{
    public class IfcSurfaceCurveSweptAreaSolidBuilder<T> : IfcSweptAreaSolidBuilder<T>,
        IIfcSurfaceCurveSweptAreaSolidBuilder<T>
        where T : IIfcSurfaceCurveSweptAreaSolid, IInstantiableEntity
    {
        public IfcSurfaceCurveSweptAreaSolidBuilder(IIfcProfileDef profileDef, IIfcCurve directrix,
            IIfcSurface referenceSurface, IfcParameterValue startParam, IfcParameterValue endParam)
            : base(profileDef)
        {
            Directrix = directrix;
            ReferenceSurface = referenceSurface;
            StartParam = startParam;
            EndParam = endParam;
        }

        public IIfcCurve Directrix { get; }
        public IIfcSurface ReferenceSurface { get; }

        public IfcParameterValue StartParam { get; }
        public IfcParameterValue EndParam { get; }

        public override T CreateSolidModel(IModel model)
        {
            T solid = base.CreateSolidModel(model);
            solid.Directrix = Directrix;
            solid.ReferenceSurface = ReferenceSurface;
            solid.StartParam = StartParam;
            solid.EndParam = EndParam;
            return solid;
        }
    }
}