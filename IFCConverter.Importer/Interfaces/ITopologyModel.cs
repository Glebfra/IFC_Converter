using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    /// <summary>
    ///     Represents a topology model that manages topology entities and their
    ///     relationships within the imported model.
    /// </summary>
    internal interface ITopologyModel
    {
        /// <summary>
        ///     Gets all topology entities contained in the model.
        /// </summary>
        public IEnumerable<ITopologyEntity> Entities { get; }

        /// <summary>
        ///     Adds a new entity proxy to the topology model.
        /// </summary>
        /// <param name="proxy">
        ///     The entity proxy to add.
        /// </param>
        /// <remarks>
        ///     The boundary is automatically resolved before the corresponding
        ///     topology entity is created and added to the model.
        ///     Connections are recalculated after the entity is added.
        /// </remarks>
        public void AddEntity(IEntityProxy proxy);

        /// <summary>
        ///     Adds a boundary proxy to the topology model.
        /// </summary>
        /// <param name="boundaryProxy">
        ///     The boundary proxy to add.
        /// </param>
        /// <remarks>
        ///     The boundary proxy is converted into its corresponding topology entity
        ///     before being added to the model.
        /// </remarks>
        public void AddEntity(IBoundaryProxy boundaryProxy);

        /// <summary>
        ///     Adds an existing topology entity to the model.
        /// </summary>
        /// <param name="entity">
        ///     The topology entity to add.
        /// </param>
        /// <remarks>
        ///     Connectivity is recalculated after the entity has been added.
        /// </remarks>
        public void AddEntity(ITopologyEntity entity);

        /// <summary>
        ///     Adds multiple entity proxies to the topology model.
        /// </summary>
        /// <param name="proxies">
        ///     The entity proxies to add.
        /// </param>
        /// <remarks>
        ///     Boundaries are resolved using both the existing entities and the
        ///     provided proxies to ensure consistent connectivity.
        ///     Connections are recalculated after all entities have been added.
        /// </remarks>
        public void AddEntities(IEnumerable<IEntityProxy> proxies);

        /// <summary>
        ///     Adds multiple boundary proxies to the topology model.
        /// </summary>
        /// <param name="boundaryProxies">
        ///     The boundary proxies to add.
        /// </param>
        public void AddEntities(IEnumerable<IBoundaryProxy> boundaryProxies);

        /// <summary>
        ///     Adds multiple topology entities to the model.
        /// </summary>
        /// <param name="entities">
        ///     The topology entities to add.
        /// </param>
        /// <remarks>
        ///     Connectivity is recalculated after all entities have been added.
        /// </remarks>
        public void AddEntities(IEnumerable<ITopologyEntity> entities);
    }
}