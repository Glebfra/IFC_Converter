using System;
using Newtonsoft.Json;

namespace Start.StartProperties
{
    public class StartEnumPropertyJsonConverter<T> : JsonConverter<T>
        where T : Enum
    {
        public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
        {
            int intValue = (int)Convert.ChangeType(value, typeof(int));
            writer.WriteValue(intValue);
        }

        public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum) 
                throw new ArgumentException("T must be an enumerated type");
            if (reader.Value == null)
                throw new NullReferenceException(nameof(reader.Value));
            int intRawValue = Convert.ToInt32(reader.Value);
            string rawValue = Convert.ToString(intRawValue);
            return (T)Enum.Parse(enumType, rawValue);
        }
    }
}