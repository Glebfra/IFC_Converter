using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFCConverter.Attributes;
using IFCConverter.Interfaces;

namespace IFCConverter.PropertySets
{
    internal class AbstractPropertySet : IPropertySet
    {
        public Dictionary<string, object> GetDictionary()
        {
            return GetType().GetFields().ToDictionary(
                GetFieldName, 
                info => info.GetValue(this)
            );
        }

        public void SetDictionary(Dictionary<string, object> dictionary)
        {
            foreach (FieldInfo field in GetType().GetFields())
            {
                string fieldName = GetFieldName(field);
                if (dictionary.TryGetValue(fieldName, out object value))
                    SetField(field, value);
            }
        }

        private string GetFieldName(FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttribute<PropertyAttribute>()?.Name ?? fieldInfo.Name;
        }

        private void SetField(FieldInfo fieldInfo, object value)
        {
            try
            {
                PropertyAttribute? propertyAttribute = fieldInfo.GetCustomAttribute<PropertyAttribute>();
                Type? typeConverterType = propertyAttribute?.TypeConverter;
                
                object convertedValue;
                if (typeConverterType != null)
                {
                    IPropertyConverter converter = (IPropertyConverter)Activator.CreateInstance(typeConverterType);
                    convertedValue = converter.Read(value);
                }
                else
                {
                    convertedValue = Convert.ChangeType(value.ToString(), fieldInfo.FieldType);
                }
                
                fieldInfo.SetValue(this, convertedValue);
            }
            
            catch (InvalidCastException)
            {
                return;
            }
        }
    }
}