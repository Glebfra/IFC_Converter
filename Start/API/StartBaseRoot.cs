using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Start.Interfaces;

namespace Start.API
{
    /// <summary>
    ///     Represents the base root class for interacting with the Start API.
    ///     Provides methods for accessing and manipulating data and connections.
    /// </summary>
    public class StartBaseRoot : IStartBaseRoot, IDisposable
    {
        private readonly object _startBaseRoot;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartBaseRoot" /> class.
        /// </summary>
        /// <param name="startBaseRoot">The underlying object representing the StartBaseRoot.</param>
        public StartBaseRoot(object startBaseRoot)
        {
            _startBaseRoot = startBaseRoot;
        }

        /// <summary>
        ///     Gets the unique identifier of the StartBaseRoot instance.
        /// </summary>
        public int Id => GetNumber();

        /// <summary>
        ///     Releases the resources used by the StartBaseRoot.
        /// </summary>
        public void Dispose()
        {
            Marshal.ReleaseComObject(_startBaseRoot);
        }

        /// <summary>
        ///     Retrieves the underlying object of the StartBaseRoot.
        /// </summary>
        /// <returns>The underlying object.</returns>
        public object GetObject()
        {
            return _startBaseRoot;
        }

        /// <summary>
        ///     Retrieves the name of the StartBaseRoot as an integer.
        /// </summary>
        /// <returns>The name as an integer.</returns>
        public int GetName()
        {
            object name = _startBaseRoot.GetType().InvokeMember(
                "GetName", BindingFlags.InvokeMethod, null, _startBaseRoot, null
            )!;
            return (int)name;
        }

        /// <summary>
        ///     Sets the name of the StartBaseRoot.
        /// </summary>
        /// <param name="name">The name to set.</param>
        public void SetName(string name)
        {
            object[] args = { name };
            _startBaseRoot.GetType().InvokeMember(
                "SetName", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }

        /// <summary>
        ///     Retrieves the number of connections associated with the StartBaseRoot.
        /// </summary>
        /// <returns>The number of connections.</returns>
        public int GetNumberConn()
        {
            object element = _startBaseRoot.GetType().InvokeMember(
                "GetNumberConn", BindingFlags.InvokeMethod, null, _startBaseRoot, null
            )!;
            return (int)element;
        }

        /// <summary>
        ///     Retrieves a connected element of a specific type and number.
        /// </summary>
        /// <param name="type">The type of the element.</param>
        /// <param name="nNumber">The number of the element.</param>
        /// <returns>The connected element as a <see cref="StartBaseRoot" />.</returns>
        public IStartBaseRoot GetConnElemOnType(StartElementTypeEnum type, int nNumber)
        {
            object element = new();
            object[] args = { type, nNumber, element };

            ParameterModifier parameterModifier = new(3) { [2] = true };
            ParameterModifier[] modifiers = { parameterModifier };

            _startBaseRoot.GetType().InvokeMember(
                "GetConnElemOnType", BindingFlags.InvokeMethod, null, _startBaseRoot, args, modifiers, null, null
            );

            return new StartBaseRoot(args[2]);
        }

        /// <summary>
        ///     Retrieves a connected element by its index.
        /// </summary>
        /// <param name="nNumber">The index of the element.</param>
        /// <returns>The connected element as a <see cref="StartBaseRoot" />.</returns>
        public IStartBaseRoot GetConnElemOnIndex(int nNumber)
        {
            object element = new();
            object[] args = { nNumber, element };

            ParameterModifier parameterModifier = new(2) { [1] = true };
            ParameterModifier[] modifiers = { parameterModifier };

            _startBaseRoot.GetType().InvokeMember(
                "GetConnElemOnIndex", BindingFlags.InvokeMethod, null, _startBaseRoot, args, modifiers, null, null
            );

            return new StartBaseRoot(args[1]);
        }

        /// <summary>
        ///     Sets a connected element by its index.
        /// </summary>
        /// <param name="index">The index of the element to set.</param>
        public void SetConnElem(int index)
        {
            object[] args = { index };
            _startBaseRoot.GetType().InvokeMember(
                "SetConnElem", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }

        /// <summary>
        ///     Retrieves the starting node of the StartBaseRoot.
        /// </summary>
        /// <returns>The starting node as a <see cref="StartBaseRoot" />.</returns>
        public IStartBaseRoot GetSNode()
        {
            object element = _startBaseRoot.GetType().InvokeMember(
                "GetSNode", BindingFlags.InvokeMethod, null, _startBaseRoot, null
            )!;
            return new StartBaseRoot(element);
        }

        /// <summary>
        ///     Retrieves the ending node of the StartBaseRoot.
        /// </summary>
        /// <returns>The ending node as a <see cref="StartBaseRoot" />.</returns>
        public IStartBaseRoot GetENode()
        {
            object element = _startBaseRoot.GetType().InvokeMember(
                "GetENode", BindingFlags.InvokeMethod, null, _startBaseRoot, null
            )!;
            return new StartBaseRoot(element);
        }

        /// <summary>
        ///     Sets the starting node by its index.
        /// </summary>
        /// <param name="index">The index of the starting node.</param>
        public void SetSNode(int index)
        {
            object[] args = { index };
            _startBaseRoot.GetType().InvokeMember(
                "SetSNode", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }

        /// <summary>
        ///     Sets the ending node by its index.
        /// </summary>
        /// <param name="index">The index of the ending node.</param>
        public void SetENode(int index)
        {
            object[] args = { index };
            _startBaseRoot.GetType().InvokeMember(
                "SetENode", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }

        /// <summary>
        ///     Sets the starting node using another <see cref="StartBaseRoot" /> instance.
        /// </summary>
        /// <param name="node">The starting node to set.</param>
        public void SetSNode(StartBaseRoot node)
        {
            object[] args = { node._startBaseRoot };
            _startBaseRoot.GetType().InvokeMember(
                "SetSNode", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }

        /// <summary>
        ///     Sets the ending node using another <see cref="StartBaseRoot" /> instance.
        /// </summary>
        /// <param name="node">The ending node to set.</param>
        public void SetENode(StartBaseRoot node)
        {
            object[] args = { node._startBaseRoot };
            _startBaseRoot.GetType().InvokeMember(
                "SetENode", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }

        /// <summary>
        ///     Retrieves a character data value by its key.
        /// </summary>
        /// <param name="key">The key of the data.</param>
        /// <returns>The character data value.</returns>
        public string GetDataChar(int key)
        {
            object[] args = { key };
            object data = _startBaseRoot.GetType().InvokeMember(
                "GetDataChar", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
            return (string)data;
        }

        /// <summary>
        ///     Retrieves an integer data value by its key.
        /// </summary>
        /// <param name="key">The key of the data.</param>
        /// <returns>The integer data value.</returns>
        public int GetDataInt(int key)
        {
            object[] args = { key };
            object data = _startBaseRoot.GetType().InvokeMember(
                "GetDataInt", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
            return (int)data;
        }

        /// <summary>
        ///     Retrieves a real (double) data value by its key.
        /// </summary>
        /// <param name="key">The key of the data.</param>
        /// <returns>The real data value.</returns>
        public double GetDataReal(int key)
        {
            object[] args = { key };
            object data = _startBaseRoot.GetType().InvokeMember(
                "GetDataReal", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
            return (double)data;
        }

        /// <summary>
        ///     Retrieves the unique number of the StartBaseRoot.
        /// </summary>
        /// <returns>The unique number.</returns>
        public int GetNumber()
        {
            object element = _startBaseRoot.GetType().InvokeMember(
                "GetNumber", BindingFlags.InvokeMethod, null, _startBaseRoot, null
            )!;
            return (int)element;
        }

        /// <summary>
        ///     Retrieves the X-coordinate of the StartBaseRoot.
        /// </summary>
        /// <param name="mode">The mode for retrieving the coordinate (default is 0).</param>
        /// <returns>The X-coordinate.</returns>
        public double GetXCoord(int mode = 0)
        {
            object[] args = { mode };
            object value = _startBaseRoot.GetType().InvokeMember(
                "GetCoordX", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            )!;
            return (double)value;
        }

        /// <summary>
        ///     Retrieves the Y-coordinate of the StartBaseRoot.
        /// </summary>
        /// <param name="mode">The mode for retrieving the coordinate (default is 0).</param>
        /// <returns>The Y-coordinate.</returns>
        public double GetYCoord(int mode = 0)
        {
            object[] args = { mode };
            object value = _startBaseRoot.GetType().InvokeMember(
                "GetCoordY", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            )!;
            return (double)value;
        }

        /// <summary>
        ///     Retrieves the Z-coordinate of the StartBaseRoot.
        /// </summary>
        /// <param name="mode">The mode for retrieving the coordinate (default is 0).</param>
        /// <returns>The Z-coordinate.</returns>
        public double GetZCoord(int mode = 0)
        {
            object[] args = { mode };
            object value = _startBaseRoot.GetType().InvokeMember(
                "GetCoordZ", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            )!;
            return (double)value;
        }

        /// <summary>
        ///     Retrieves JSON data associated with the StartBaseRoot.
        /// </summary>
        /// <param name="mode">The mode for retrieving the data (default is 0).</param>
        /// <param name="key">The key for retrieving the data (default is 0).</param>
        /// <returns>The JSON data as a string.</returns>
        public string GetDataJson(int mode = 0, int key = 0)
        {
            object[] args = { mode, key };
            object value = _startBaseRoot.GetType().InvokeMember(
                "GetDataJson", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            )!;
            return (string)value;
        }

        /// <summary>
        ///     Sets JSON data for the StartBaseRoot.
        /// </summary>
        /// <param name="key">The key for the data.</param>
        /// <param name="data">The JSON data to set.</param>
        public void SetDataJson(int key, string data)
        {
            object[] args = { key, data };
            _startBaseRoot.GetType().InvokeMember(
                "SetDataJson", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }
    }
}