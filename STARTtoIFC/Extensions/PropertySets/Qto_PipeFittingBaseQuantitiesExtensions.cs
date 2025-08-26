using IFC.PropertySets;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Ifc4.MeasureResource;

namespace STARTtoIFC.Extensions.PropertySets
{
    internal class Qto_PipeFittingBaseQuantitiesExtensions
    {
        public static Qto_PipeFittingBaseQuantities CreateFromStart(StartAbstractFittingEntity fittingEntity)
        {
            Qto_PipeFittingBaseQuantities qto = new Qto_PipeFittingBaseQuantities()
            {
                NetWeight = new ActionProperty<IfcMassMeasure>(fittingEntity.Weight.SIProperty),
            };

            return qto;
        }
    }
}