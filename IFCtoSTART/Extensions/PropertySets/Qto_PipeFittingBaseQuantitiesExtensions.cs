using IFC.PropertySets;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.QuantityResource;

namespace IFCtoSTART.Extensions.PropertySets
{
    internal static class Qto_PipeFittingBaseQuantitiesExtensions
    {
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