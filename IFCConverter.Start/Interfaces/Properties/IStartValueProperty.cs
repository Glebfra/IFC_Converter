using System;

namespace IFCConverter.Start.Interfaces
{
    public interface IStartValueProperty : IStartProperty
    {
        bool HasValue { get; }
        object GetStartProperty();
        object GetSIProperty();
        string GetStartUnit();
        string GetSIUnit();
    }

    /// <summary>
    ///     Represents a property in the IFCConverter.Start system with associated units and type information.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public interface IStartValueProperty<T> : IStartValueProperty, IComparable
        where T : struct, IComparable<T>
    {
        /// <summary>
        ///     Gets the value of the property in the IFCConverter.Start system's units.
        /// </summary>
        T StartProperty { get; }

        /// <summary>
        ///     Gets the value of the property in SI (International System of Units) units.
        /// </summary>
        T SIProperty { get; }

        /// <summary>
        ///     Gets the start to SI factor.
        /// </summary>
        double StartToSIFactor { get; }

        /// <summary>
        ///     Gets the unit of the property in the IFCConverter.Start system.
        /// </summary>
        string StartUnit { get; }

        /// <summary>
        ///     Gets the unit of the property in SI (International System of Units).
        /// </summary>
        string SIUnit { get; }

        /// <summary>
        ///     Retrieves the type of the property value.
        /// </summary>
        /// <returns>The <see cref="Type" /> of the property value.</returns>
        Type GetGenericType();

        /// <summary>
        ///     Creates the property value from the specified IFCConverter.Start system value.
        /// </summary>
        /// <param name="startProperty">The value in the IFCConverter.Start system's units.</param>
        IStartValueProperty<T> CreateFromStart(T startProperty);

        /// <summary>
        ///     Creates the property value from the specified SI (International System of Units) value.
        /// </summary>
        /// <param name="siProperty">The value in SI units.</param>
        IStartValueProperty<T> CreateFromSI(T siProperty);
    }
}