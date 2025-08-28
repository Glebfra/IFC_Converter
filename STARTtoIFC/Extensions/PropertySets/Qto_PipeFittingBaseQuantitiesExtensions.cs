using IFC.PropertySets;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.QuantityResource;

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
        
        public static Qto_PipeFittingBaseQuantities CreateFromPropertySet(IIfcElementQuantity propertySet)
        {
            Qto_PipeFittingBaseQuantities qto = new Qto_PipeFittingBaseQuantities();
            foreach (IIfcPhysicalQuantity quantity in propertySet.Quantities)
            {
                switch (quantity.Name)
                {
                    case nameof(qto.Length):
                        qto.Length = ((IfcQuantityLength)quantity).LengthValue;
                        break;
                    case nameof(qto.NetWeight):
                        qto.NetWeight = ((IfcQuantityWeight)quantity).WeightValue;
                        break;
                }
            }

            return qto;
        }
    }
}