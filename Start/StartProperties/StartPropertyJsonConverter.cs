using System;
using Newtonsoft.Json;

namespace Start.StartProperties
{
    internal class StartPropertyJsonConverter<T, U> : JsonConverter<T>
    { 
        public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
        {
            StartAbstractProperty<U> abstractProperty = value as StartAbstractProperty<U>;
            writer.WriteValue(abstractProperty.StartProperty);
        }

        public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                throw new NullReferenceException(nameof(reader.Value));
            return (T)Activator.CreateInstance(typeof(T), reader.Value);
        }
    }
}