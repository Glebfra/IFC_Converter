using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Start.API;
using Start.StartProperties;

namespace Start.Entities.Abstract
{
    public abstract class StartAbstractEntity
    {
        [JsonIgnore] 
        public int ID;
        
        [JsonIgnore]
        public StartElementType Type = StartElementType.ALL;

        public Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                object? value = property.GetValue(this);
                switch (value)
                {
                    case null:
                        continue;
                    case IStartProperty<double> startProperty:
                        dictionary.Add(property.Name, $"{startProperty.SIProperty} {startProperty.SIUnit}");
                        break;
                    case double startProperty:
                        dictionary.Add(property.Name, $"{startProperty}");
                        break;
                    case string startProperty:
                        dictionary.Add(property.Name, startProperty);
                        break;
                    default:
                        dictionary.Add(property.Name, value.ToString());
                        break;
                }
            }

            return dictionary;
        }
    }
}