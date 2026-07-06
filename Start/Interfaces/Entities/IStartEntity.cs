using System.Collections.Generic;

namespace Start.Interfaces
{
    /// <summary>
    ///     Defines the base contract for all domain entities within the Start system.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Every entity in the Start domain model must implement this interface.
    ///         It provides:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Basic identification (ID, Name)</description>
    ///         </item>
    ///         <item>
    ///             <description>Graph connectivity via <see cref="ConnectedEntities" /></description>
    ///         </item>
    ///         <item>
    ///             <description>Structured metadata export via <see cref="GetData" /></description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Entities form a graph structure where connections represent
    ///         physical, logical, or topological relationships (e.g., pipes connected to nodes,
    ///         fittings connected to segments, etc.).
    ///     </para>
    /// </remarks>
    public interface IStartEntity
    {
        /// <summary>
        ///     Gets or sets the display name of the entity.
        /// </summary>
        /// <remarks>
        ///     The name is typically used for UI representation and human-readable identification.
        ///     It does not have to be unique unless enforced by higher-level logic.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier of the entity within the project scope.
        /// </summary>
        /// <remarks>
        ///     The ID is typically assigned during deserialization or entity creation.
        ///     For node-based elements, it may correspond to a node index.
        /// </remarks>
        public int ID { get; set; }

        /// <summary>
        ///     Gets or sets the collection of entities connected to this entity.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This property represents graph relationships between entities.
        ///         Examples:
        ///     </para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>Pipe → connected nodes</description>
        ///         </item>
        ///         <item>
        ///             <description>Reducer → connected pipe segments</description>
        ///         </item>
        ///         <item>
        ///             <description>Node → connected elements</description>
        ///         </item>
        ///     </list>
        ///     <para>
        ///         The list should be kept synchronized during graph construction.
        ///     </para>
        /// </remarks>
        public List<IStartEntity> ConnectedEntities { get; set; }

        /// <summary>
        ///     Returns structured key-value data representing entity properties.
        /// </summary>
        /// <returns>
        ///     A dictionary containing property names and their string representations.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method is typically used for:
        ///     </para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>Serialization</description>
        ///         </item>
        ///         <item>
        ///             <description>Logging</description>
        ///         </item>
        ///         <item>
        ///             <description>Debugging</description>
        ///         </item>
        ///         <item>
        ///             <description>Export to external formats</description>
        ///         </item>
        ///     </list>
        ///     <para>
        ///         Implementations should ensure that all relevant domain properties
        ///         are included in the returned dictionary.
        ///     </para>
        /// </remarks>
        public IDictionary<string, string> GetData();
    }
}