using System;
using System.Diagnostics.Contracts;
using IFCConverter.Start.API;

namespace IFCConverter.Start.Interfaces
{
    public interface IStartProject : IDisposable
    {
        /// <summary>
        ///     Performs post-import visualization adjustments.
        ///     Sets view orientation and zoom.
        /// </summary>
        void OnImportFinish();

        /// <summary>
        ///     Returns connected entities of specified type.
        /// </summary>
        /// <param name="entity">Source entity.</param>
        /// <param name="type">Element type filter.</param>
        /// <returns>Array of connected entities.</returns>
        [Pure]
        IStartBaseRoot[] GetConnEntities(IStartBaseRoot entity, StartElementTypeEnum type);

        /// <summary>
        ///     Returns first connected entity of specified type.
        /// </summary>
        /// <param name="entity">Source entity.</param>
        /// <param name="type">Element type filter.</param>
        /// <returns>Connected entity.</returns>
        [Pure]
        IStartBaseRoot GetConnEntity(IStartBaseRoot entity, StartElementTypeEnum type);

        /// <summary>
        ///     Returns all entities within specified type range.
        /// </summary>
        /// <param name="minType">Minimum element type.</param>
        /// <param name="maxType">Maximum element type.</param>
        /// <returns>Array of entities.</returns>
        [Pure]
        IStartBaseRoot[] GetEntities(StartElementTypeEnum minType, StartElementTypeEnum maxType);

        /// <summary>
        ///     Returns number of elements within specified type range.
        /// </summary>
        [Pure]
        int GetNumberElements(StartElementTypeEnum minType, StartElementTypeEnum maxType);

        StartEntityProxy AddEntity(IStartEntity entity);

        /// <summary>
        ///     Adds new element of specified type.
        /// </summary>
        /// <param name="type">Element type.</param>
        /// <param name="index">Output index of created element.</param>
        /// <returns>Created <see cref="StartBaseRoot" />.</returns>
        IStartBaseRoot AddElement(StartElementTypeEnum type, out int index);

        /// <summary>
        ///     Adds new element and connects it to start and end nodes.
        /// </summary>
        IStartBaseRoot AddElementAndNode(StartElementTypeEnum type, int nSNode, int nENode, out int index);

        /// <summary>
        ///     Adds new element connected to single node.
        /// </summary>
        IStartBaseRoot AddElementAndNode(StartElementTypeEnum type, int nSNode, out int index);

        /// <summary>
        ///     Returns full project data serialized to JSON.
        /// </summary>
        /// <returns>JSON string representation of all elements.</returns>
        [Pure]
        string GetDataJson();

        IStartEntity[] GetStartEntities();
    }
}