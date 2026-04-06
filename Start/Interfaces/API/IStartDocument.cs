using System;
using Start.API;

namespace Start.Interfaces
{
    public interface IStartDocument : IDisposable
    {
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
            StartManufacturingTechnologyEnum manufacturingTechnologyEnum, double thickness, int nElem, double temp);

        /// <summary>
        ///     Retrieves the data array dispatch object from the document.
        /// </summary>
        /// <returns>A <see cref="IStartBaseRootDataArray" /> representing the data array dispatch.</returns>
        public IStartBaseRootDataArray GetDataArrayDispatch();

        /// <summary>
        ///     Sets the view of the model using the specified view parameters.
        /// </summary>
        /// <param name="view">An array of integers representing the view parameters.</param>
        public void SetViewOfModel(int[] view);

        /// <summary>
        ///     Draws the entire view of the model.
        /// </summary>
        public void DrawViewAll();

        /// <summary>
        ///     Adjusts the view to fit all elements in the model.
        /// </summary>
        public void DrawFitAll();

        /// <summary>
        ///     Retrieves the title of the document.
        /// </summary>
        /// <returns>A string representing the title of the document.</returns>
        public string GetTitle();

        /// <summary>
        ///     Retrieves the file path of the document.
        /// </summary>
        /// <returns>A string representing the file path of the document.</returns>
        public string GetPathName();
    }
}