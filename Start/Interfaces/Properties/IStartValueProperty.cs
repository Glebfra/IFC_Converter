using System;

namespace Start.Interfaces
{
    /// <summary>
    ///     Represents a property in the Start system with associated units and type information.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public interface IStartValueProperty<T> : IStartProperty, IComparable
        where T : struct, IComparable<T>
    {
        /// <summary>
        ///     Gets the value of the property in the Start system's units.
        /// </summary>
        public T StartProperty { get; }

        /// <summary>
        ///     Gets the value of the property in SI (International System of Units) units.
        /// </summary>
        public T SIProperty { get; }

        /// <summary>
        ///     Gets the start to SI factor.
        /// </summary>
        public double StartToSIFactor { get; }

        /// <summary>
        ///     Gets the unit of the property in the Start system.
        /// </summary>
        public string StartUnit { get; }

        /// <summary>
        ///     Gets the unit of the property in SI (International System of Units).
        /// </summary>
        public string SIUnit { get; }

        /// <summary>
        ///     Retrieves the type of the property value.
        /// </summary>
        /// <returns>The <see cref="Type" /> of the property value.</returns>
        public Type GetGenericType();

        /// <summary>
        ///     Creates the property value from the specified Start system value.
        /// </summary>
        /// <param name="startProperty">The value in the Start system's units.</param>
        public IStartValueProperty<T> CreateFromStart(T startProperty);

        /// <summary>
        ///     Creates the property value from the specified SI (International System of Units) value.
        /// </summary>
        /// <param name="siProperty">The value in SI units.</param>
        public IStartValueProperty<T> CreateFromSI(T siProperty);
    }
}