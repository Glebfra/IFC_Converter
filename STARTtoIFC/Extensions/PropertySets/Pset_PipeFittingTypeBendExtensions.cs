using IFC.PropertySets;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Ifc4.MeasureResource;

namespace STARTtoIFC.Extensions.PropertySets
{
    internal static class Pset_PipeFittingTypeBendExtensions
    {
        public static Pset_PipeFittingTypeBend CreateFromStart(StartBendEntity bendEntity)
        {
            return new Pset_PipeFittingTypeBend()
            {
                BendRadius = new ActionProperty<IfcPositiveLengthMeasure>(bendEntity.Radius.SIProperty),
            };
        }
    }
}