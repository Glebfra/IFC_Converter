using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.PropertySets;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.Extensions
{
    internal static class IfcRootExtensions
    {
        [Pure]
        public static Dictionary<string, object> ToDictionary(this IIfcPropertySet ifcPropertySet)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            foreach (IIfcProperty hasProperty in ifcPropertySet.HasProperties)
            {
                if (hasProperty is IIfcPropertySingleValue singleValue)
                    properties[singleValue.Name] = singleValue.NominalValue;
                if (hasProperty is IIfcPropertyListValue listValue)
                    properties[listValue.Name] = listValue.ListValues;
            }

            return properties;
        }
        
        [Pure]
        public static IEnumerable<IPropertySet> GetPropertySets(this IfcProduct product)
        {
            PropertySetRegistry registry = PropertySetRegistry.GetInstance();
            return product.PropertySets.Select(set => registry.Read<AbstractPropertySet>(set));
        }
    }
}