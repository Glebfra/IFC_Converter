using IFC.PropertySets;
using Start.Entities.Abstract;

namespace STARTtoIFC.Extensions.PropertySets
{
    #if NEW
    
    internal class Qto_PipeFittingBaseQuantitiesExtensions
    {
        public static Qto_PipeFittingBaseQuantities CreateFromStart(StartAbstractFittingEntity fittingEntity)
        {
            Qto_PipeFittingBaseQuantities qto = new Qto_PipeFittingBaseQuantities()
            {
                NetWeight = fittingEntity.Weight.SIProperty,
            };

            return qto;
        }
    }
    
    #endif
}