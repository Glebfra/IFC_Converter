using System;
using Start.API;

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

        public Type? StartEntityAugmenterType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartElementAttribute" /> class with the specified element type.
        /// </summary>
        /// <param name="type">The type of the Start element.</param>
        /// <param name="startEntityAugmenterType">The entity augmenter type</param>
        public StartElementAttribute(StartElementTypeEnum type, Type? startEntityAugmenterType = null)
        {
            Type = type;
            StartEntityAugmenterType = startEntityAugmenterType;
        }
    }
}