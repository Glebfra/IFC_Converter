using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Start.Interfaces
{
    /// <summary>
    ///     Represents a spatial node entity in the IFCConverter.Start domain model.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A node defines a point in three-dimensional space and serves as a
    ///         topological anchor for connected entities such as pipes, fittings,
    ///         and other structural elements.
    ///     </para>
    ///     <para>
    ///         Node entities are fundamental building blocks of the geometric graph.
    ///         Other entities derive their spatial configuration from connected nodes.
    ///     </para>
    /// </remarks>
    public interface IStartNodeEntity : IStartEntity
    {
        /// <summary>
        ///     Gets or sets the textual description of the node.
        /// </summary>
        /// <remarks>
        ///     May contain additional contextual or engineering information
        ///     such as elevation reference, installation notes, or constraints.
        /// </remarks>
        string Description { get; set; }

        /// <summary>
        ///     Gets or sets the X coordinate of the node.
        /// </summary>
        /// <remarks>
        ///     The value is wrapped in <see cref="IStartValueProperty{T}" />
        ///     to support validation, metadata, units, or change tracking.
        /// </remarks>
        IStartValueProperty<double> XCoord { get; set; }

        /// <summary>
        ///     Gets or sets the Y coordinate of the node.
        /// </summary>
        /// <remarks>
        ///     The value is wrapped in <see cref="IStartValueProperty{T}" />
        ///     to support validation, metadata, units, or change tracking.
        /// </remarks>
        IStartValueProperty<double> YCoord { get; set; }

        /// <summary>
        ///     Gets or sets the Z coordinate of the node.
        /// </summary>
        /// <remarks>
        ///     The value is wrapped in <see cref="IStartValueProperty{T}" />
        ///     to support validation, metadata, units, or change tracking.
        /// </remarks>
        IStartValueProperty<double> ZCoord { get; set; }

        /// <summary>
        ///     Gets the three-dimensional position vector of the node.
        /// </summary>
        /// <value>
        ///     A <see cref="Vector{Double}" /> representing (X, Y, Z) coordinates.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The position vector is typically constructed from
        ///         <see cref="XCoord" />, <see cref="YCoord" />, and <see cref="ZCoord" />.
        ///     </para>
        ///     <para>
        ///         Implementations should ensure consistency between
        ///         coordinate properties and the returned vector.
        ///     </para>
        ///     <para>
        ///         The vector is commonly used in:
        ///     </para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>Geometric calculations</description>
        ///         </item>
        ///         <item>
        ///             <description>Projection and direction computations</description>
        ///         </item>
        ///         <item>
        ///             <description>Distance and alignment analysis</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        Vector<double> Position { get; set; }
    }
}