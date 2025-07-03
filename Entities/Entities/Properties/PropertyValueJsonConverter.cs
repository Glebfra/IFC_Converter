using System;
using Newtonsoft.Json;

namespace Entities.Entities.Properties
{
    /// <summary>
    /// This class declares a right way to convert object properties from start json
    /// </summary>
    /// <typeparam name="T">Inherited property class from IStartProperty</typeparam>
    /// <typeparam name="U">Inherited property type (double, int, etc...)</typeparam>
    internal class PropertyValueJsonConverter<T, U> : JsonConverter<T>
        where T : IProperty<U>
    { 
        public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
        {
            if (value is not IProperty<U> abstractProperty) 
                throw new NullReferenceException(nameof(abstractProperty));
            writer.WriteValue(abstractProperty.StartProperty);
        }

        public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                throw new NullReferenceException(nameof(reader.Value));
            return (T)Activator.CreateInstance(typeof(T), Convert.ChangeType(reader.Value, typeof(U)));
        }
    }
}