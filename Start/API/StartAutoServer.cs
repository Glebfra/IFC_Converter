using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Start.Interfaces;

namespace Start.API
{
    /// <summary>
    ///     Represents a wrapper for interacting with the CTAPT AutoServer COM object.
    /// </summary>
    public class StartAutoServer : IStartAutoServer, IDisposable
    {
        private const string PROG_ID = "CTAPT.AutoServer";
        private readonly object _autoServer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartAutoServer" /> class by creating the AutoServer COM object.
        /// </summary>
        /// <exception cref="Exception">Thrown when the AutoServer COM object cannot be found or instantiated.</exception>
        public StartAutoServer()
        {
            Type? type = Type.GetTypeFromProgID(PROG_ID);
            _autoServer = type != null
                ? Activator.CreateInstance(type)
                : throw new Exception($"Cannot find the prog_id: {PROG_ID}");
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartAutoServer" /> class with an existing AutoServer COM object.
        /// </summary>
        /// <param name="autoServer">The existing AutoServer COM object.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided AutoServer object is null.</exception>
        public StartAutoServer(object autoServer)
        {
            _autoServer = autoServer ?? throw new ArgumentNullException(nameof(autoServer));
        }

        /// <summary>
        ///     Loads a document into the AutoServer.
        /// </summary>
        /// <param name="mode">The mode in which to load the document.</param>
        /// <param name="filepath">The file path of the document to load.</param>
        /// <returns>A <see cref="StartDocument" /> representing the loaded document.</returns>
        public IStartDocument LoadStartDocument(int mode, string filepath)
        {
            object document = LoadStartDocumentRaw(mode, filepath);
            return new StartDocument(document);
        }

        /// <summary>
        ///     Loads a document into the AutoServer and returns the raw object.
        /// </summary>
        /// <param name="mode">The mode in which to load the document.</param>
        /// <param name="filepath">The file path of the document to load.</param>
        /// <returns>The raw document object.</returns>
        public object LoadStartDocumentRaw(int mode, string filepath)
        {
            object? document = _autoServer.GetType().InvokeMember(
                "LoadCTAPTDocument", BindingFlags.InvokeMethod, null, _autoServer, new object[] { mode, filepath, 0 }
            );
            return document;
        }

        /// <summary>
        ///     Retrieves the material JSON string from the AutoServer.
        /// </summary>
        /// <param name="nNorma">The norma value.</param>
        /// <param name="material">The material name.</param>
        /// <param name="manufacturingTechnologyEnum">The manufacturing technology enumeration value.</param>
        /// <param name="thickness">The thickness of the material.</param>
        /// <param name="nElem">The element type.</param>
        /// <param name="temp">The temperature value.</param>
        /// <returns>A JSON string representing the material.</returns>
        /// <exception cref="Exception">Thrown when the material cannot be found.</exception>
        public string GetMaterialJson(int nNorma, string material,
            StartManufacturingTechnologyEnum manufacturingTechnologyEnum, double thickness, int nElem, double temp)
        {
            object[] args = { nNorma, material, (int)manufacturingTechnologyEnum, thickness, nElem, temp };
            object? materialJson = _autoServer.GetType().InvokeMember(
                "GetMaterialJson", BindingFlags.InvokeMethod, null, _autoServer, args
            );
            if (materialJson == null)
                throw new Exception("Cannot find start material");
            return (string)materialJson;
        }

        /// <summary>
        ///     Saves the current state of the AutoServer to a file.
        /// </summary>
        /// <param name="filepath">The file path where the state should be saved.</param>
        public void SaveToFile(string filepath)
        {
            object[] args = { filepath };
            _autoServer.GetType().InvokeMember("SaveToFile", BindingFlags.InvokeMethod, null, _autoServer, args);
        }

        /// <summary>
        ///     Retrieves the full name of the AutoServer.
        /// </summary>
        /// <returns>The full name of the AutoServer, or null if not available.</returns>
        public string? GetFullName()
        {
            object? fullName = _autoServer.GetType().InvokeMember(
                "FullName", BindingFlags.InvokeMethod, null, _autoServer, null
            );
            return (string?)fullName;
        }

        /// <summary>
        ///     Retrieves the data array dispatch object from the AutoServer.
        /// </summary>
        /// <returns>A <see cref="StartBaseRootDataArray" /> representing the data array dispatch.</returns>
        public IStartBaseRootDataArray GetDataArrayDispatch()
        {
            object? dataArray = _autoServer.GetType().InvokeMember(
                "GetDataArrayDispatch", BindingFlags.InvokeMethod, null, _autoServer, null
            );
            return new StartBaseRootDataArray(dataArray);
        }

        /// <summary>
        ///     Releases the resources used by the AutoServer.
        /// </summary>
        public void Dispose()
        {
            Marshal.ReleaseComObject(_autoServer);
        }
    }
}