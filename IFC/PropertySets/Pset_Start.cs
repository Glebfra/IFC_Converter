using System.Collections.Generic;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class Pset_Start : IPropertySet
    {
        public Dictionary<string, string> Data;

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = nameof(Pset_Start);
                foreach (KeyValuePair<string, string> kvp in Data)
                {
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = kvp.Key;
                        value.NominalValue = new IfcText(kvp.Value);
                    }));
                }
            });
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