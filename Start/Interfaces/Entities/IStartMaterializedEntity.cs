namespace Start.Interfaces
{
    /// <summary>
    ///     Represents an entity that has an associated material definition.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This interface extends <see cref="IStartEntity" /> by introducing
    ///         material information required for physical, analytical, or visualization purposes.
    ///     </para>
    ///     <para>
    ///         Materialized entities typically participate in:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Strength and stress calculations</description>
    ///         </item>
    ///         <item>
    ///             <description>Thermal or hydraulic analysis</description>
    ///         </item>
    ///         <item>
    ///             <description>Weight and mass estimation</description>
    ///         </item>
    ///         <item>
    ///             <description>Rendering and visualization</description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Implementations should ensure that the material name corresponds
    ///         to a valid material definition in the project or external material library.
    ///     </para>
    /// </remarks>
    public interface IStartMaterializedEntity : IStartEntity
    {
        /// <summary>
        ///     Gets or sets the name of the material assigned to the entity.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The material name is typically used as a lookup key in:
        ///     </para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>Material databases</description>
        ///         </item>
        ///         <item>
        ///             <description>Project-level material registries</description>
        ///         </item>
        ///         <item>
        ///             <description>External simulation systems</description>
        ///         </item>
        ///     </list>
        ///     <para>
        ///         This property should not be null or empty in valid domain objects.
        ///         Validation rules are expected to be enforced at the application layer.
        ///     </para>
        /// </remarks>
        public string MaterialName { get; set; }
    }
}