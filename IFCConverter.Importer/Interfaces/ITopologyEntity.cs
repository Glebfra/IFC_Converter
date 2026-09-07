using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    /// <summary>
    ///     Represents a topology entity that can be connected to other topology entities through topology nodes and converted
    ///     to a START entity.
    /// </summary>
    internal interface ITopologyEntity
    {
        /// <summary>
        ///     Gets the boundary proxy associated with this topology entity.
        /// </summary>
        IBoundaryProxy Proxy { get; }

        /// <summary>
        ///     Gets the topology entities that are directly connected to this entity.
        /// </summary>
        IReadOnlyCollection<ITopologyEntity> Connected { get; }

        /// <summary>
        ///     Gets the topology nodes that define this entity.
        /// </summary>
        IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; }

        /// <summary>
        ///     Creates a connection between this entity and the specified topology entity.
        /// </summary>
        /// <param name="topologyEntity">
        ///     The topology entity to connect.
        /// </param>
        void Connect(ITopologyEntity topologyEntity);

        /// <summary>
        ///     Creates connections between this entity and the specified topology entities.
        /// </summary>
        /// <param name="topologyEntities">
        ///     The topology entities to connect.
        /// </param>
        void Connect(IEnumerable<ITopologyEntity> topologyEntities);

        /// <summary>
        ///     Removes the connection between this entity and the specified topology entity.
        /// </summary>
        /// <param name="topologyEntity">
        ///     The topology entity to disconnect.
        /// </param>
        void Disconnect(ITopologyEntity topologyEntity);

        /// <summary>
        ///     Removes the connections between this entity and the specified topology entities.
        /// </summary>
        /// <param name="topologyEntities">
        ///     The topology entities to disconnect.
        /// </param>
        void Disconnect(IEnumerable<ITopologyEntity> topologyEntities);

        /// <summary>
        ///     Converts this topology entity to its corresponding START entity.
        /// </summary>
        /// <returns>
        ///     The converted START entity.
        /// </returns>
        [Pure]
        IStartEntity ToStartEntity();
    }
}