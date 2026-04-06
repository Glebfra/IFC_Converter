using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Start.Interfaces;

namespace Start.API
{
    /// <summary>
    ///     Represents a wrapper for the StartBaseRootDataArray object, providing methods to interact with its elements and
    ///     connections.
    /// </summary>
    public class StartBaseRootDataArray : IStartBaseRootDataArray, IDisposable
    {
        private readonly object _startBaseRootDataArray;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartBaseRootDataArray" /> class.
        /// </summary>
        /// <param name="startBaseRootDataArray">The underlying StartBaseRootDataArray object.</param>
        public StartBaseRootDataArray(object startBaseRootDataArray)
        {
            _startBaseRootDataArray = startBaseRootDataArray;
        }

        /// <summary>
        ///     Releases the resources used by the StartBaseRootDataArray.
        /// </summary>
        public void Dispose()
        {
            Marshal.ReleaseComObject(_startBaseRootDataArray);
        }

        /// <summary>
        ///     Adds a new element of the specified type to the array.
        /// </summary>
        /// <param name="type">The type of the element to add.</param>
        /// <param name="index">The index of the added element.</param>
        /// <returns>The added element.</returns>
        public object AddElement(StartElementTypeEnum type, out int index)
        {
            object element = new();
            object[] args = { type, element };

            ParameterModifier parameterModifier = new(2) { [1] = true };
            ParameterModifier[] modifiers = { parameterModifier };

            index = (int)_startBaseRootDataArray.GetType().InvokeMember(
                "AddElement", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args, modifiers, null, null
            );

            return args[1];
        }

        /// <summary>
        ///     Adds a new element with specified nodes to the array.
        /// </summary>
        /// <param name="type">The type of the element to add.</param>
        /// <param name="nSNode">The starting node of the element.</param>
        /// <param name="nENode">The ending node of the element.</param>
        /// <param name="index">The index of the added element.</param>
        /// <returns>The added element.</returns>
        public object AddElementAndNode(StartElementTypeEnum type, int nSNode, int nENode, out int index)
        {
            object element = new();
            object[] args = { type, nSNode, nENode, element };

            ParameterModifier parameterModifier = new(4) { [3] = true };
            ParameterModifier[] modifiers = { parameterModifier };

            index = (int)_startBaseRootDataArray.GetType().InvokeMember(
                "AddElementAndNode", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args, modifiers, null,
                null
            );

            return args[3];
        }

        /// <summary>
        ///     Adds a new element with a specified starting node to the array.
        /// </summary>
        /// <param name="type">The type of the element to add.</param>
        /// <param name="nSNode">The starting node of the element.</param>
        /// <param name="index">The index of the added element.</param>
        /// <returns>The added element.</returns>
        public object AddElementAndNode(StartElementTypeEnum type, int nSNode, out int index)
        {
            object element = new();
            object[] args = { type, nSNode, element };

            ParameterModifier parameterModifier = new(3) { [2] = true };
            ParameterModifier[] modifiers = { parameterModifier };

            index = (int)_startBaseRootDataArray.GetType().InvokeMember(
                "AddElementAndNode", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args, modifiers, null,
                null
            );

            return args[2];
        }

        /// <summary>
        ///     Deletes an element from the array by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the element to delete.</param>
        public void DeleteElement(int id)
        {
            object[] args = { id };
            _startBaseRootDataArray.GetType().InvokeMember("DeleteElement", BindingFlags.InvokeMethod, null,
                _startBaseRootDataArray, args);
        }

        /// <summary>
        ///     Sets a JSON data block for the array.
        /// </summary>
        /// <param name="mode">The mode for setting the data block.</param>
        /// <param name="json">The JSON data to set.</param>
        public void SetDataBlockJson(int mode, string json)
        {
            object[] args = { mode, json };
            _startBaseRootDataArray.GetType().InvokeMember(
                "SetDataBlockJson", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
            );
        }

        /// <summary>
        ///     Retrieves an element dispatch object by its identifier and type range.
        /// </summary>
        /// <param name="id">The identifier of the element.</param>
        /// <param name="minType">The minimum type of the element.</param>
        /// <param name="maxType">The maximum type of the element.</param>
        /// <returns>The retrieved element as a <see cref="StartBaseRoot" />.</returns>
        public IStartBaseRoot GetElementDispatch(int id, StartElementTypeEnum minType, StartElementTypeEnum maxType)
        {
            object element = new();
            object[] args = { id, minType, maxType, element };

            ParameterModifier parameterModifier = new(4) { [3] = true };
            ParameterModifier[] modifiers = { parameterModifier };

            _startBaseRootDataArray.GetType().InvokeMember(
                "GetElementDispatch", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args, modifiers, null,
                null
            );

            return new StartBaseRoot(args[3]);
        }

        /// <summary>
        ///     Retrieves a connection dispatch object by its identifier and connection number.
        /// </summary>
        /// <param name="id">The identifier of the connection.</param>
        /// <param name="nNumber">The connection number.</param>
        /// <returns>The retrieved connection as a <see cref="StartBaseRoot" />.</returns>
        public IStartBaseRoot GetConnDispatch(int id, int nNumber)
        {
            object element = new();
            object[] args = { id, nNumber, element };

            ParameterModifier parameterModifier = new(3) { [2] = true };
            ParameterModifier[] modifiers = { parameterModifier };

            _startBaseRootDataArray.GetType().InvokeMember(
                "GetConnDispatch", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args, modifiers, null, null
            );

            return new StartBaseRoot(args[2]);
        }

        /// <summary>
        ///     Retrieves the number of elements within a specified type range.
        /// </summary>
        /// <param name="minType">The minimum type of the elements (default is ALL).</param>
        /// <param name="maxType">The maximum type of the elements (default is ALL).</param>
        /// <returns>The number of elements.</returns>
        public int GetNumberElements(StartElementTypeEnum minType = StartElementTypeEnum.ALL,
            StartElementTypeEnum maxType = StartElementTypeEnum.ALL)
        {
            object[] args = { minType, maxType };
            object? elementsNumber = _startBaseRootDataArray.GetType().InvokeMember(
                "GetNumberElements", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
            );

            return (int)elementsNumber;
        }

        /// <summary>
        ///     Retrieves the number of connections for a specific element within a type range.
        /// </summary>
        /// <param name="id">The identifier of the element.</param>
        /// <param name="minType">The minimum type of the connections.</param>
        /// <param name="maxType">The maximum type of the connections.</param>
        /// <returns>The number of connections.</returns>
        public int GetNumberConns(int id, StartElementTypeEnum minType, StartElementTypeEnum maxType)
        {
            object[] args = { id, minType, maxType };
            object? elementsNumber = _startBaseRootDataArray.GetType().InvokeMember(
                "GetNumberConns", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
            );
            return (int)elementsNumber;
        }

        /// <summary>
        ///     Retrieves the title of an element by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the element.</param>
        /// <returns>The title of the element.</returns>
        public string GetTitle(int id)
        {
            object[] args = { id };
            object? title = _startBaseRootDataArray.GetType().InvokeMember(
                "GetTitle", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
            );

            return (string)title;
        }

        /// <summary>
        ///     Retrieves JSON data for elements within a specified type range.
        /// </summary>
        /// <param name="minType">The minimum type of the elements.</param>
        /// <param name="maxType">The maximum type of the elements.</param>
        /// <param name="mode">The mode for retrieving the data (default is 0).</param>
        /// <returns>The JSON data as a string.</returns>
        public string GetDataJson(StartElementTypeEnum minType, StartElementTypeEnum maxType, int mode = 0)
        {
            object[] args = { mode, minType, maxType };
            object? dataJson = _startBaseRootDataArray.GetType().InvokeMember(
                "GetInputDataJsonArray", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
            );

            return (string)dataJson ?? "{}";
        }
    }
}