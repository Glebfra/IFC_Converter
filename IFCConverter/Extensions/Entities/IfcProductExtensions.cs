using System.Collections.Generic;
using IFC.PropertySets;
using IFCConverter.Extensions.PropertySets;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Extensions.Entities
{
    internal static class IfcProductExtensions
    {
        public static IEnumerable<IPropertySet> GetPropertySets(this IfcProduct product)
        {
            List<IPropertySet> propertySets = new List<IPropertySet>();
            foreach (IIfcPropertySet productPropertySet in product.PropertySets)
            {
                if (productPropertySet.Name == null)
                    continue;
                
                if (productPropertySet.Name.ToString().Contains(nameof(AVEVA_Pset)))
                    propertySets.Add(AVEVA_PsetExtensions.CreateFromPropertySet(productPropertySet));
                
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
                    case nameof(AVEVA_EntityParameters):
                        propertySets.Add(AVEVA_EntityParametersExtensions.CreateFromPropertySet(productPropertySet));
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