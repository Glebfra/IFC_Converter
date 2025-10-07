using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Fittings.Vertex;
using IFC.Entities.Segments;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;

namespace IFCConverter.Importers
{
    internal interface IImporter
    {
        /// <summary>
        /// Retrieves an array of IfcPipeSegment objects from the provided IfcProduct array.
        /// </summary>
        /// <param name="products">An array of IfcProduct objects to extract pipe segments from.</param>
        /// <returns>An array of IfcProduct objects.</returns>
        public IfcElement[] GetPipeSegments(IfcProduct[] products);

        /// <summary>
        /// Retrieves an array of IfcPipeFitting objects representing bends from the provided IfcProduct array.
        /// </summary>
        /// <param name="products">An array of IfcProduct objects to extract bends from.</param>
        /// <returns>An array of IfcProduct objects representing bends.</returns>
        public IfcElement[] GetBends(IfcProduct[] products);

        /// <summary>
        /// Retrieves an array of IfcPipeFitting objects representing tees from the provided IfcProduct array.
        /// </summary>
        /// <param name="products">An array of IfcProduct objects to extract tees from.</param>
        /// <returns>An array of IfcProduct objects representing tees.</returns>
        public IfcElement[] GetTees(IfcProduct[] products);
        
        /// <summary>
        /// Retrieves an array of IfcPipeFitting objects representing tees from the provided IfcProduct array.
        /// </summary>
        /// <param name="products">An array of IfcProduct objects to extract tees from.</param>
        /// <returns>An array of IfcProduct objects representing reducers.</returns>
        public IfcElement[] GetReducers(IfcProduct[] products);
        
        /// <summary>
        /// Creates an array of IfcPipeSegmentEntity objects from the provided IfcPipeSegment array.
        /// </summary>
        /// <param name="pipes">An array of IfcElement objects to convert into entities.</param>
        /// <returns>An array of IfcPipeSegmentEntity objects.</returns>
        public IfcPipeSegmentEntity[] CreatePipeSegments(IfcElement[] pipes);
        
        /// <summary>
        /// Creates an array of IfcCadBendEntity objects from the provided bends and abstract segment entities.
        /// </summary>
        /// <param name="bends">An array of IfcElement objects representing bends.</param>
        /// <param name="abstractSegmentEntities">An array of IfcAbstractSegmentEntity objects to associate with the bends.</param>
        /// <returns>An array of IfcCadBendEntity objects.</returns>
        public IfcCadBendEntity[] CreateBends(IfcElement[] bends, IfcAbstractSegmentEntity[] abstractSegmentEntities);
        
        /// <summary>
        /// Creates an array of IfcWeldedTeeEntity objects from the provided tees and abstract segment entities.
        /// </summary>
        /// <param name="tees">An array of IfcElement objects representing tees.</param>
        /// <param name="abstractSegmentEntities">An array of IfcAbstractSegmentEntity objects to associate with the tees.</param>
        /// <returns>An array of IfcWeldedTeeEntity objects.</returns>
        public IfcWeldedTeeEntity[] CreateWeldedTees(IfcElement[] tees, IfcAbstractSegmentEntity[] abstractSegmentEntities);

        /// <summary>
        /// Creates an array of IfcWeldedTeeEntity objects from the provided tees and abstract segment entities.
        /// </summary>
        /// <param name="reducers">An array of IfcElement objects representing reducers.</param>
        /// <param name="abstractSegmentEntities">An array of IfcAbstractSegmentEntity objects to associate with the reducers.</param>
        /// <returns>An array of IfcVertexReducerEccentricEntity objects.</returns>
        public IfcVertexReducerEccentricEntity[] CreateReducers(IfcElement[] reducers, IfcAbstractSegmentEntity[] abstractSegmentEntities);
    }
}