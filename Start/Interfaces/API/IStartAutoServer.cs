using System;
using Start.API;

namespace Start.Interfaces
{
    public interface IStartAutoServer : IDisposable
    {
        /// <summary>
        ///     Loads a document into the AutoServer.
        /// </summary>
        /// <param name="mode">The mode in which to load the document.</param>
        /// <param name="filepath">The file path of the document to load.</param>
        /// <returns>A <see cref="IStartDocument" /> representing the loaded document.</returns>
        public IStartDocument LoadStartDocument(int mode, string filepath);

        /// <summary>
        ///     Loads a document into the AutoServer and returns the raw object.
        /// </summary>
        /// <param name="mode">The mode in which to load the document.</param>
        /// <param name="filepath">The file path of the document to load.</param>
        /// <returns>The raw document object.</returns>
        public object LoadStartDocumentRaw(int mode, string filepath);

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
            StartManufacturingTechnologyEnum manufacturingTechnologyEnum, double thickness, int nElem, double temp);

        /// <summary>
        ///     Saves the current state of the AutoServer to a file.
        /// </summary>
        /// <param name="filepath">The file path where the state should be saved.</param>
        public void SaveToFile(string filepath);

        /// <summary>
        ///     Retrieves the full name of the AutoServer.
        /// </summary>
        /// <returns>The full name of the AutoServer, or null if not available.</returns>
        public string? GetFullName();

        /// <summary>
        ///     Retrieves the data array dispatch object from the AutoServer.
        /// </summary>
        /// <returns>A <see cref="IStartBaseRootDataArray" /> representing the data array dispatch.</returns>
        public IStartBaseRootDataArray GetDataArrayDispatch();
    }
}