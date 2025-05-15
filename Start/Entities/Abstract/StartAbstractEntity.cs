using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Fittings;
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
            AddToDictionary(dictionary, GetType(), this);
            return dictionary;
        }

        private static void AddToDictionary(IDictionary<string, string> dictionary, Type type, object @object, string? propertyName = null)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                object? value = propertyInfo.GetValue(@object);
                string newPropertyName = propertyName != null ? $"{propertyName}_{propertyInfo.Name}" : propertyInfo.Name;
                switch (value)
                {
                    case null:
                        continue;
                    case IStartProperty<double> startProperty:
                        dictionary.Add(newPropertyName, $"{startProperty.SIProperty} {startProperty.SIUnit}");
                        break;
                    case IStartProperty<int> startProperty:
                        dictionary.Add(newPropertyName, $"{startProperty.SIProperty} {startProperty.SIUnit}");
                        break;
                    case StartNonStandardRestraintModule restraintModule:
                        AddToDictionary(dictionary, restraintModule.GetType(), restraintModule, newPropertyName);
                        break;
                    default:
                        dictionary.Add(newPropertyName, value.ToString());
                        break;
                }
            }
        }
    }
}