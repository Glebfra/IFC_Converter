using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IFC.PropertySets;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFCtoSTART.Extensions.PropertySets
{
    internal static class Pset_StartExtensions
    {
        public static double GetDoublePropertyValue(string rawValue)
        {
            Regex regex = new Regex(@"-(\d+,\d+)|-(\d+.\d+)|-\d+|(\d+,\d+)|(\d+.\d+)|\d+");
            Match match = regex.Match(rawValue);
            return Convert.ToDouble(match.Value);
        }

        public static int GetIntPropertyValue(string rawValue)
        {
            Regex regex = new Regex(@"-(\d+)|\d+");
            Match match = regex.Match(rawValue);
            return Convert.ToInt32(match.Value);
        }
        
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