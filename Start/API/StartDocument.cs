using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Start.Interfaces;

namespace Start.API
{
    /// <summary>
    ///     Represents a document in the Start API, providing methods to interact with its data and views.
    /// </summary>
    public class StartDocument : IStartDocument, IDisposable
    {
        private readonly object _document;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartDocument" /> class with the specified document object.
        /// </summary>
        /// <param name="document">The underlying document object.</param>
        public StartDocument(object document)
        {
            _document = document;
        }

        /// <summary>
        ///     Retrieves the material JSON string based on the specified parameters.
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
            object? materialJson = _document.GetType().InvokeMember(
                "GetMaterialJson", BindingFlags.InvokeMethod, null, _document, args
            );
            if (materialJson == null)
                throw new Exception("Cannot find start material");
            return (string)materialJson;
        }

        /// <summary>
        ///     Retrieves the data array dispatch object from the document.
        /// </summary>
        /// <returns>A <see cref="StartBaseRootDataArray" /> representing the data array dispatch.</returns>
        public IStartBaseRootDataArray GetDataArrayDispatch()
        {
            object? dataArray = _document.GetType().InvokeMember(
                "GetDataArrayDispatch", BindingFlags.InvokeMethod, null, _document, null
            );
            return new StartBaseRootDataArray(dataArray);
        }

        /// <summary>
        ///     Sets the view of the model using the specified view parameters.
        /// </summary>
        /// <param name="view">An array of integers representing the view parameters.</param>
        public void SetViewOfModel(int[] view)
        {
            object[] args = view.Cast<object>().ToArray();
            _document.GetType().InvokeMember("SetViewOfModel", BindingFlags.InvokeMethod, null, _document, args);
        }

        /// <summary>
        ///     Draws the entire view of the model.
        /// </summary>
        public void DrawViewAll()
        {
            _document.GetType().InvokeMember("DrawViewAll", BindingFlags.InvokeMethod, null, _document, null);
        }

        /// <summary>
        ///     Adjusts the view to fit all elements in the model.
        /// </summary>
        public void DrawFitAll()
        {
            _document.GetType().InvokeMember("DrawFitAll", BindingFlags.InvokeMethod, null, _document, null);
        }

        /// <summary>
        ///     Retrieves the title of the document.
        /// </summary>
        /// <returns>A string representing the title of the document.</returns>
        public string GetTitle()
        {
            object? title = _document.GetType().InvokeMember(
                "GetTitle", BindingFlags.InvokeMethod, null, _document, null
            );
            return (string)title;
        }

        /// <summary>
        ///     Retrieves the file path of the document.
        /// </summary>
        /// <returns>A string representing the file path of the document.</returns>
        public string GetPathName()
        {
            object? title = _document.GetType().InvokeMember(
                "GetPathName", BindingFlags.InvokeMethod, null, _document, null
            );
            return (string)title;
        }

        /// <summary>
        ///     Releases the resources used by the document.
        /// </summary>
        public void Dispose()
        {
            if (_document != null)
                Marshal.ReleaseComObject(_document);
        }
    }
}