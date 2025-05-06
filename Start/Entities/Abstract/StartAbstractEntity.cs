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
        public int ID { get; set; }
        
        [JsonIgnore]
        public StartElementType Type { get; set; } = StartElementType.ALL;

        public Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (PropertyInfo field in GetType().GetProperties())
            {
                object? value = field.GetValue(this);
                switch (value)
                {
                    case null:
                        continue;
                    case IStartProperty<double> startProperty:
                        dictionary.Add(field.Name, $"{startProperty.SIProperty} {startProperty.SIUnit}");
                        break;
                    case double startProperty:
                        dictionary.Add(field.Name, $"{startProperty}");
                        break;
                    case string startProperty:
                        dictionary.Add(field.Name, startProperty);
                        break;
                }
            }

            return dictionary;
        }
    }
}