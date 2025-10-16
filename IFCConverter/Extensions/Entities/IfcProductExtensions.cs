using System.Collections.Generic;
using System.Linq;
using IFC.PropertySets;
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
                    propertySets.Add(AVEVA_Pset.CreateFromPropertySet(productPropertySet));
                
                switch (productPropertySet.Name)
                {
                    case nameof(Pset_Start):
                        propertySets.Add(Pset_Start.CreateFromPropertySet(productPropertySet));
                        break;
                    case nameof(Pset_PipeFittingTypeBend):
                        propertySets.Add(Pset_PipeFittingTypeBend.CreateFromPropertySet(productPropertySet));
                        break;
                    case nameof(Pset_PipeFittingTypeJunction):
                        propertySets.Add(Pset_PipeFittingTypeBend.CreateFromPropertySet(productPropertySet));
                        break;
                    case nameof(Pset_PipeSegmentTypeCommon):
                        propertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromPropertySet(productPropertySet));
                        break;
                    case nameof(AVEVA_EntityParameters):
                        propertySets.Add(AVEVA_EntityParameters.CreateFromPropertySet(productPropertySet));
                        break;
                }
            }

            foreach (IIfcElementQuantity productElementQuantity in product.ElementQuantities)
            {
                switch (productElementQuantity.Name)
                {
                    case nameof(Qto_PipeSegmentBaseQuantities):
                        propertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromQuantitySet(productElementQuantity));
                        break;
                    case nameof(Qto_PipeFittingBaseQuantities):
                        propertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromPropertySet(productElementQuantity));
                        break;
                }
            }

            return propertySets;
        }

        public static IEnumerable<IIfcRepresentationItem> GetRepresentationItems(this IfcProduct product)
        {
            return product.Representation.Representations
                .SelectMany(representation => representation.Items);
        }
    }
}