using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using IFCConverter.Importer.Attributes;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Importer.PropertySets
{
    internal class AbstractPropertySet : IPropertySet
    {
        [Pure]
        public Dictionary<string, object> GetDictionary()
        {
            return GetType().GetFields().ToDictionary(
                GetFieldName,
                info => info.GetValue(this)
            );
        }

        [Pure]
        public Dictionary<string, object> GetNotNullDictionary()
        {
            return GetType()
                .GetFields()
                .Select(info => new { Info = info, Value = info.GetValue(this) })
                .Where(obj => obj.Value != null)
                .ToDictionary(obj => GetFieldName(obj.Info), obj => obj.Value);
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

        [Pure]
        private string GetFieldName(FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttribute<PropertyAttribute>()?.Name ?? fieldInfo.Name;
        }

        private void SetField(FieldInfo fieldInfo, object value)
        {
            try
            {
                PropertyAttribute propertyAttribute = fieldInfo.GetCustomAttribute<PropertyAttribute>();
                Type typeConverterType = propertyAttribute?.TypeConverter;

                object convertedValue;
                if (typeConverterType != null)
                {
                    IPropertyConverter converter = ParameterlessConstructorRegistry<IPropertyConverter>.Create(typeConverterType);
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
            }
        }
    }
}