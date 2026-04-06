using System;
using Start.API;

namespace Start.Interfaces
{
    public interface IStartBaseRoot : IDisposable
    {
        /// <summary>
        ///     Gets the unique identifier of the StartBaseRoot instance.
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     Retrieves the underlying object of the StartBaseRoot.
        /// </summary>
        /// <returns>The underlying object.</returns>
        public object GetObject();

        /// <summary>
        ///     Retrieves the name of the StartBaseRoot as an integer.
        /// </summary>
        /// <returns>The name as an integer.</returns>
        public int GetName();

        /// <summary>
        ///     Sets the name of the StartBaseRoot.
        /// </summary>
        /// <param name="name">The name to set.</param>
        public void SetName(string name);

        /// <summary>
        ///     Retrieves the number of connections associated with the StartBaseRoot.
        /// </summary>
        /// <returns>The number of connections.</returns>
        public int GetNumberConn();

        /// <summary>
        ///     Retrieves a connected element of a specific type and number.
        /// </summary>
        /// <param name="type">The type of the element.</param>
        /// <param name="nNumber">The number of the element.</param>
        /// <returns>The connected element as a <see cref="IStartBaseRoot" />.</returns>
        public IStartBaseRoot GetConnElemOnType(StartElementTypeEnum type, int nNumber);

        /// <summary>
        ///     Retrieves a connected element by its index.
        /// </summary>
        /// <param name="nNumber">The index of the element.</param>
        /// <returns>The connected element as a <see cref="IStartBaseRoot" />.</returns>
        public IStartBaseRoot GetConnElemOnIndex(int nNumber);

        /// <summary>
        ///     Sets a connected element by its index.
        /// </summary>
        /// <param name="index">The index of the element to set.</param>
        public void SetConnElem(int index);

        /// <summary>
        ///     Retrieves the starting node of the StartBaseRoot.
        /// </summary>
        /// <returns>The starting node as a <see cref="IStartBaseRoot" />.</returns>
        public IStartBaseRoot GetSNode();

        /// <summary>
        ///     Retrieves the ending node of the StartBaseRoot.
        /// </summary>
        /// <returns>The ending node as a <see cref="IStartBaseRoot" />.</returns>
        public IStartBaseRoot GetENode();

        /// <summary>
        ///     Sets the starting node by its index.
        /// </summary>
        /// <param name="index">The index of the starting node.</param>
        public void SetSNode(int index);

        /// <summary>
        ///     Sets the ending node by its index.
        /// </summary>
        /// <param name="index">The index of the ending node.</param>
        public void SetENode(int index);

        /// <summary>
        ///     Sets the starting node using another <see cref="StartBaseRoot" /> instance.
        /// </summary>
        /// <param name="node">The starting node to set.</param>
        public void SetSNode(StartBaseRoot node);

        /// <summary>
        ///     Sets the ending node using another <see cref="StartBaseRoot" /> instance.
        /// </summary>
        /// <param name="node">The ending node to set.</param>
        public void SetENode(StartBaseRoot node);

        /// <summary>
        ///     Retrieves a character data value by its key.
        /// </summary>
        /// <param name="key">The key of the data.</param>
        /// <returns>The character data value.</returns>
        public string GetDataChar(int key);

        /// <summary>
        ///     Retrieves an integer data value by its key.
        /// </summary>
        /// <param name="key">The key of the data.</param>
        /// <returns>The integer data value.</returns>
        public int GetDataInt(int key);

        /// <summary>
        ///     Retrieves a real (double) data value by its key.
        /// </summary>
        /// <param name="key">The key of the data.</param>
        /// <returns>The real data value.</returns>
        public double GetDataReal(int key);

        /// <summary>
        ///     Retrieves the unique number of the StartBaseRoot.
        /// </summary>
        /// <returns>The unique number.</returns>
        public int GetNumber();

        /// <summary>
        ///     Retrieves the X-coordinate of the StartBaseRoot.
        /// </summary>
        /// <param name="mode">The mode for retrieving the coordinate (default is 0).</param>
        /// <returns>The X-coordinate.</returns>
        public double GetXCoord(int mode = 0);

        /// <summary>
        ///     Retrieves the Y-coordinate of the StartBaseRoot.
        /// </summary>
        /// <param name="mode">The mode for retrieving the coordinate (default is 0).</param>
        /// <returns>The Y-coordinate.</returns>
        public double GetYCoord(int mode = 0);

        /// <summary>
        ///     Retrieves the Z-coordinate of the StartBaseRoot.
        /// </summary>
        /// <param name="mode">The mode for retrieving the coordinate (default is 0).</param>
        /// <returns>The Z-coordinate.</returns>
        public double GetZCoord(int mode = 0);

        /// <summary>
        ///     Retrieves JSON data associated with the StartBaseRoot.
        /// </summary>
        /// <param name="mode">The mode for retrieving the data (default is 0).</param>
        /// <param name="key">The key for retrieving the data (default is 0).</param>
        /// <returns>The JSON data as a string.</returns>
        public string GetDataJson(int mode = 0, int key = 0);

        /// <summary>
        ///     Sets JSON data for the StartBaseRoot.
        /// </summary>
        /// <param name="key">The key for the data.</param>
        /// <param name="data">The JSON data to set.</param>
        public void SetDataJson(int key, string data);
    }
}