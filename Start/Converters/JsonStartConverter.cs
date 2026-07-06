using System;
using Newtonsoft.Json;
using Start.Interfaces;

namespace Start.Converters
{
    /// <summary>
    ///     A custom JSON converter for serializing and deserializing objects of type <typeparamref name="TStart" />.
    /// </summary>
    /// <typeparam name="TStart">The type of the object to be converted, which must implement <see cref="IStartProperty" />.</typeparam>
    public class JsonStartConverter<TStart> : JsonConverter<TStart>
        where TStart : IStartProperty, new()
    {
        /// <summary>
        ///     Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter" /> to write to.</param>
        /// <param name="value">The object value to write.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="NullReferenceException">Thrown when the value is null.</exception>
        public override void WriteJson(JsonWriter writer, TStart? value, JsonSerializer serializer)
        {
            if (value == null)
                throw new NullReferenceException(nameof(value));
            writer.WriteValue(value?.Write());
        }

        /// <summary>
        ///     Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader" /> to read from.</param>
        /// <param name="objectType">The type of the object to create.</param>
        /// <param name="existingValue">The existing value of the object being read.</param>
        /// <param name="hasExistingValue">Indicates whether there is an existing value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The deserialized object of type <typeparamref name="TStart" />.</returns>
        /// <exception cref="NullReferenceException">Thrown when the reader value is null.</exception>
        public override TStart? ReadJson(JsonReader reader, Type objectType, TStart? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                throw new NullReferenceException(nameof(reader.Value));
            if (existingValue != null)
            {
                existingValue.Read(reader.Value);
                return existingValue;
            }

            TStart startProperty = new();
            startProperty.Read(reader.Value);
            return startProperty;
        }
    }
}