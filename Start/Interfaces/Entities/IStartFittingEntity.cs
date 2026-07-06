namespace Start.Interfaces
{
    /// <summary>
    ///     Represents a fitting entity within the Start domain model.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A fitting is a one-node element that typically connects
    ///         linear elements (e.g., pipes) and modifies flow direction,
    ///         diameter, or topology.
    ///     </para>
    ///     <para>
    ///         This interface combines:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see cref="IStartEntity" /> — base entity functionality
    ///                 (identification, connectivity, metadata).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="IStartOneNodeEntity" /> — spatial behavior
    ///                 defined by a single reference node.
    ///             </description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Typical implementations include reducers, bends, tees,
    ///         valves, and other connection components.
    ///     </para>
    ///     <para>
    ///         In the entity graph, a fitting:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Is connected to exactly one primary node</description>
    ///         </item>
    ///         <item>
    ///             <description>May be connected to multiple pipe segments</description>
    ///         </item>
    ///         <item>
    ///             <description>May influence geometry recalculation of adjacent elements</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public interface IStartFittingEntity : IStartEntity, IStartOneNodeEntity
    {
    }
}