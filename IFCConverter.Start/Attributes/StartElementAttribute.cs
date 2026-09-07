using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Start.API;
using IFCConverter.Start.Interfaces.Augmenters;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Start.Attributes
{
    /// <summary>
    ///     Specifies that a class or struct represents a IFCConverter.Start element of a specific type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    public class StartElementAttribute : Attribute
    {

        public Type[] StartEntityAugmenterTypes;

        /// <summary>
        ///     Gets or sets the type of the IFCConverter.Start element.
        /// </summary>
        public StartElementTypeEnum Type;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartElementAttribute" /> class with the specified element type.
        /// </summary>
        /// <param name="type">The type of the IFCConverter.Start element.</param>
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