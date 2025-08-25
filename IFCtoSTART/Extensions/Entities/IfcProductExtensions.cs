using System.Collections.Generic;
using IFC.PropertySets;
using IFCtoSTART.Extensions.PropertySets;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCtoSTART.Extensions.Entities
{
    internal static class IfcProductExtensions
    {
        public static IEnumerable<IPropertySet> GetPropertySets(this IfcProduct product)
        {
            List<IPropertySet> propertySets = new List<IPropertySet>();
            foreach (IIfcPropertySet productPropertySet in product.PropertySets)
            {
                switch (productPropertySet.Name)
                {
                    case nameof(Pset_Start):
                        propertySets.Add(Pset_StartExtensions.CreateFromPropertySet(productPropertySet));
                        break;
                    case nameof(Pset_PipeFittingTypeBend):
                        propertySets.Add(Pset_PipeFittingTypeBendExtensions.CreateFromPropertySet(productPropertySet));
                        break;
                    case nameof(Pset_PipeFittingTypeJunction):
                        propertySets.Add(Pset_PipeFittingTypeBendExtensions.CreateFromPropertySet(productPropertySet));
                        break;
                    case nameof(Pset_PipeSegmentTypeCommon):
                        propertySets.Add(Pset_PipeSegmentTypeCommonExtensions.CreateFromPropertySet(productPropertySet));
                        break;
                }
            }
            
            foreach (IIfcElementQuantity productElementQuantity in product.ElementQuantities)
            {
                switch (productElementQuantity.Name)
                {
                    case nameof(Qto_PipeSegmentBaseQuantities):
                        propertySets.Add(Qto_PipeSegmentBaseQuantitiesExtensions.CreateFromQuantitySet(productElementQuantity));
                        break;
                    case nameof(Qto_PipeFittingBaseQuantitiesExtensions):
                        propertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromPropertySet(productElementQuantity));
                        break;
                }
            }

            return propertySets;
        }
    }
}