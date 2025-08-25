using System.Collections.Generic;
using IFC.PropertySets;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFCtoSTART.Extensions.PropertySets
{
    internal static class Pset_StartExtensions
    {
        public static Pset_Start CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                IfcPropertySingleValue? singleValue = property as IfcPropertySingleValue;
                if (singleValue == null)
                    continue;
                
                string name = singleValue.Name;
                string value = (IfcText)singleValue.NominalValue;
                data.Add(name, value);
            }

            Pset_Start pset = new Pset_Start();
            pset.Data = data;

            return pset;
        }
    }
}