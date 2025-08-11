using IFC.PropertySets;
using Start.Entities.Fittings;

namespace STARTtoIFC.Extensions.PropertySets
{
    internal static class Pset_PipeFittingTypeBendExtensions
    {
        public static Pset_PipeFittingTypeBend CreateFromStart(StartBendEntity bendEntity)
        {
            return new Pset_PipeFittingTypeBend()
            {
                BendRadius = bendEntity.Radius.SIProperty,
            };
        }
    }
}