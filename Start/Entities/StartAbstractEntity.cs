using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using Start.Attributes;
using Start.Interfaces;

namespace Start.Entities
{
    /// <summary>
    ///     Represents an abstract base class for Start entities, implementing the <see cref="IStartEntity" /> interface.
    /// </summary>
    [DebuggerDisplay("Name: {Name}")]
    public abstract class StartAbstractEntity : IStartEntity
    {
        /// <summary>
        ///     Gets or sets the unique identifier of the entity.
        ///     This property is ignored during JSON serialization and by the Start framework.
        /// </summary>
        [JsonIgnore]
        [StartIgnore]
        public int ID { get; set; }

        /// <summary>
        ///     Gets or sets the list of entities connected to this entity.
        ///     This property is ignored during JSON serialization and by the Start framework.
        /// </summary>
        [JsonIgnore]
        [StartIgnore]
        public List<IStartEntity> ConnectedEntities { get; set; } = new();

        /// <summary>
        ///     Gets or sets the name of the entity.
        ///     This property must be implemented by derived classes.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        ///     Retrieves the data of the entity as a dictionary of key-value pairs.
        /// </summary>
        /// <returns>A dictionary containing the entity's data.</returns>
        public IDictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new();
            AddToDictionary(dictionary, GetType(), this);
            return dictionary;
        }

        /// <summary>
        ///     Recursively adds the properties of the specified object to the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to populate with property data.</param>
        /// <param name="type">The type of the object being processed.</param>
        /// <param name="obj">The object whose properties are being added.</param>
        /// <param name="propertyName">The optional prefix for property names.</param>
        private static void AddToDictionary(IDictionary<string, string> dictionary, Type type, object obj,
            string? propertyName = null)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                if (propertyInfo.GetCustomAttribute<StartIgnoreAttribute>() != null)
                    continue;

                object? value = propertyInfo.GetValue(obj);
                string newPropertyName =
                    propertyName != null ? $"{propertyName}_{propertyInfo.Name}" : propertyInfo.Name;
                switch (value)
                {
                    case null:
                        continue;
                    case IStartValueProperty<double> startProperty:
                        dictionary.Add(newPropertyName, $"{startProperty.SIProperty} {startProperty.SIUnit}");
                        break;
                    case IStartValueProperty<int> startProperty:
                        dictionary.Add(newPropertyName, $"{startProperty.SIProperty} {startProperty.SIUnit}");
                        break;
                    case IStartExpansionModule expansionModule:
                        AddToDictionary(dictionary, expansionModule.GetType(), expansionModule, newPropertyName);
                        break;
                    default:
                        dictionary.Add(newPropertyName, value.ToString());
                        break;
                }
            }
        }
    }
}