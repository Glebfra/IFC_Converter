using System;
using System.Collections.Generic;
using System.Linq;
using Start.API;
using Start.Interfaces.Augmenters;
using Utils;

namespace Start.Attributes
{
    /// <summary>
    ///     Specifies that a class or struct represents a Start element of a specific type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    public class StartElementAttribute : Attribute
    {
        /// <summary>
        ///     Gets or sets the type of the Start element.
        /// </summary>
        public StartElementTypeEnum Type;

        public Type[] StartEntityAugmenterTypes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartElementAttribute" /> class with the specified element type.
        /// </summary>
        /// <param name="type">The type of the Start element.</param>
        /// <param name="startEntityAugmenterTypes">The entity augmenters types</param>
        public StartElementAttribute(StartElementTypeEnum type, params Type[] startEntityAugmenterTypes)
        {
            Type = type;
            StartEntityAugmenterTypes = startEntityAugmenterTypes;
        }

        public IEnumerable<IStartEntityAugmenter> GetAugmenters()
        {
            return StartEntityAugmenterTypes.Select(ParameterlessConstructorRegistry<IStartEntityAugmenter>.Create);
        }
    }
}