using System;
using Start.API;

namespace Start.Interfaces
{
    public interface IStartBaseRootDataArray : IDisposable
    {
        /// <summary>
        ///     Adds a new element of the specified type to the array.
        /// </summary>
        /// <param name="type">The type of the element to add.</param>
        /// <param name="index">The index of the added element.</param>
        /// <returns>The added element.</returns>
        public object AddElement(StartElementTypeEnum type, out int index);

        /// <summary>
        ///     Adds a new element with specified nodes to the array.
        /// </summary>
        /// <param name="type">The type of the element to add.</param>
        /// <param name="nSNode">The starting node of the element.</param>
        /// <param name="nENode">The ending node of the element.</param>
        /// <param name="index">The index of the added element.</param>
        /// <returns>The added element.</returns>
        public object AddElementAndNode(StartElementTypeEnum type, int nSNode, int nENode, out int index);

        /// <summary>
        ///     Adds a new element with a specified starting node to the array.
        /// </summary>
        /// <param name="type">The type of the element to add.</param>
        /// <param name="nSNode">The starting node of the element.</param>
        /// <param name="index">The index of the added element.</param>
        /// <returns>The added element.</returns>
        public object AddElementAndNode(StartElementTypeEnum type, int nSNode, out int index);

        /// <summary>
        ///     Deletes an element from the array by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the element to delete.</param>
        public void DeleteElement(int id);

        /// <summary>
        ///     Sets a JSON data block for the array.
        /// </summary>
        /// <param name="mode">The mode for setting the data block.</param>
        /// <param name="json">The JSON data to set.</param>
        public void SetDataBlockJson(int mode, string json);

        /// <summary>
        ///     Retrieves an element dispatch object by its identifier and type range.
        /// </summary>
        /// <param name="id">The identifier of the element.</param>
        /// <param name="minType">The minimum type of the element.</param>
        /// <param name="maxType">The maximum type of the element.</param>
        /// <returns>The retrieved element as a <see cref="IStartBaseRoot" />.</returns>
        public IStartBaseRoot GetElementDispatch(int id, StartElementTypeEnum minType, StartElementTypeEnum maxType);

        /// <summary>
        ///     Retrieves a connection dispatch object by its identifier and connection number.
        /// </summary>
        /// <param name="id">The identifier of the connection.</param>
        /// <param name="nNumber">The connection number.</param>
        /// <returns>The retrieved connection as a <see cref="IStartBaseRoot" />.</returns>
        public IStartBaseRoot GetConnDispatch(int id, int nNumber);

        /// <summary>
        ///     Retrieves the number of elements within a specified type range.
        /// </summary>
        /// <param name="minType">The minimum type of the elements (default is ALL).</param>
        /// <param name="maxType">The maximum type of the elements (default is ALL).</param>
        /// <returns>The number of elements.</returns>
        public int GetNumberElements(StartElementTypeEnum minType = StartElementTypeEnum.ALL,
            StartElementTypeEnum maxType = StartElementTypeEnum.ALL);

        /// <summary>
        ///     Retrieves the number of connections for a specific element within a type range.
        /// </summary>
        /// <param name="id">The identifier of the element.</param>
        /// <param name="minType">The minimum type of the connections.</param>
        /// <param name="maxType">The maximum type of the connections.</param>
        /// <returns>The number of connections.</returns>
        public int GetNumberConns(int id, StartElementTypeEnum minType, StartElementTypeEnum maxType);

        /// <summary>
        ///     Retrieves the title of an element by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the element.</param>
        /// <returns>The title of the element.</returns>
        public string GetTitle(int id);

        /// <summary>
        ///     Retrieves JSON data for elements within a specified type range.
        /// </summary>
        /// <param name="minType">The minimum type of the elements.</param>
        /// <param name="maxType">The maximum type of the elements.</param>
        /// <param name="mode">The mode for retrieving the data (default is 0).</param>
        /// <returns>The JSON data as a string.</returns>
        public string GetDataJson(StartElementTypeEnum minType, StartElementTypeEnum maxType, int mode = 0);
    }
}